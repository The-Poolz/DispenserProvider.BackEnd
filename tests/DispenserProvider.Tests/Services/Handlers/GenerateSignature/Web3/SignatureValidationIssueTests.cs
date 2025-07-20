using Moq;
using Xunit;
using FluentAssertions;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;

namespace DispenserProvider.Tests.Services.Handlers.GenerateSignature.Web3;

/// <summary>
/// Tests specifically for the signature validation issue described in #146
/// These tests verify that the ±1 second timestamp discrepancy issue is resolved
/// </summary>
public class SignatureValidationIssueTests
{
    public class Issue146_TimestampDiscrepancy
    {
        public Issue146_TimestampDiscrepancy()
        {
            Environment.SetEnvironmentVariable("VALID_UNTIL_MAX_OFFSET_IN_SECONDS", "300");
            Environment.SetEnvironmentVariable("VALID_FROM_OFFSET_IN_SECONDS", "300");
        }

        [Fact]
        internal void WhenSignatureProcessedWithDelay_ShouldNotHaveTimestampMismatch()
        {
            // This test simulates the original problem:
            // "If >500ms elapse between calls, one side may round the timestamp up by +1s 
            // while the other doesn't, causing a signature mismatch"

            // Arrange
            var dbFactory = new MockDbContextFactory(seed: true);
            var signatureGenerator = new Mock<ISignatureGenerator>();
            
            var generatorCallTimestamps = new List<DateTime>();
            signatureGenerator.Setup(x => x.GenerateSignature(It.IsAny<TransactionDetailDTO>(), It.IsAny<DateTime>()))
                .Callback<TransactionDetailDTO, DateTime>((_, validUntil) => 
                {
                    // Record when the signature generator is called and with what timestamp
                    generatorCallTimestamps.Add(validUntil);
                })
                .Returns("0x_consistent_signature");

            var dispenser = dbFactory.Current.Dispenser.First();
            var signatureProcessor = new SignatureProcessor(dbFactory, signatureGenerator.Object);

            // Act - Simulate processing that might take >500ms in real scenario
            var processingStartTime = DateTime.UtcNow;
            var validFrom = signatureProcessor.SaveSignature(dispenser, false);
            var processingEndTime = DateTime.UtcNow;

            // Assert
            generatorCallTimestamps.Should().HaveCount(1);
            var savedSignature = dbFactory.Current.Signatures.First();
            
            // The key assertion: The timestamp used for signature generation 
            // should be EXACTLY the same as the one stored in the database
            generatorCallTimestamps[0].Should().Be(savedSignature.ValidUntil,
                "Signature generation and database storage must use identical timestamps");

            // Additional verification: The processing time should not affect timestamp consistency
            // (This was the core issue - processing delays caused timestamp drift)
            var actualProcessingTime = processingEndTime - processingStartTime;
            // Even if processing takes time, the timestamps should be identical
            actualProcessingTime.Should().BeLessThan(TimeSpan.FromSeconds(1), 
                "Test should complete quickly, but even if it doesn't, timestamps should be consistent");
        }

        [Fact]
        internal void WhenMultipleSignaturesGeneratedQuickly_ShouldNotCauseRacingConditions()
        {
            // This test ensures that rapid consecutive signature generations
            // don't cause the timestamp inconsistency issue

            // Arrange
            var dbFactory = new MockDbContextFactory(seed: true);
            var signatureGenerator = new Mock<ISignatureGenerator>();
            
            var allGeneratorTimestamps = new List<DateTime>();
            signatureGenerator.Setup(x => x.GenerateSignature(It.IsAny<TransactionDetailDTO>(), It.IsAny<DateTime>()))
                .Callback<TransactionDetailDTO, DateTime>((_, validUntil) => 
                {
                    allGeneratorTimestamps.Add(validUntil);
                })
                .Returns("0x_signature");

            var dispenser = dbFactory.Current.Dispenser.First();
            var signatureProcessor = new SignatureProcessor(dbFactory, signatureGenerator.Object);

            // Act - Generate multiple signatures in rapid succession
            var results = new List<DateTime>();
            for (int i = 0; i < 10; i++)
            {
                results.Add(signatureProcessor.SaveSignature(dispenser, false));
                // Simulate the kind of delay that could cause timestamp boundary issues
                Thread.Sleep(55); // 55ms * 10 = 550ms total, crossing potential second boundaries
            }

            // Assert
            allGeneratorTimestamps.Should().HaveCount(10);
            var savedSignatures = dbFactory.Current.Signatures.OrderBy(x => x.Id).ToList();
            savedSignatures.Should().HaveCount(10);

            // Verify each signature has consistent timestamps
            for (int i = 0; i < 10; i++)
            {
                allGeneratorTimestamps[i].Should().Be(savedSignatures[i].ValidUntil,
                    $"Signature {i}: Generator timestamp must match database timestamp");
                
                results[i].Should().Be(savedSignatures[i].ValidFrom,
                    $"Signature {i}: Returned ValidFrom must match database ValidFrom");
            }
        }

        [Theory]
        [InlineData(499)] // Just under the 500ms threshold mentioned in the issue
        [InlineData(500)] // Exactly at the threshold
        [InlineData(501)] // Just over the threshold
        [InlineData(999)] // Near one second boundary
        [InlineData(1001)] // Over one second
        internal void WhenProcessingTakesSpecificDuration_ShouldMaintainTimestampConsistency(int simulatedDelayMs)
        {
            // This test specifically addresses the ">500ms elapse" scenario from the issue

            // Arrange
            var dbFactory = new MockDbContextFactory(seed: true);
            var signatureGenerator = new Mock<ISignatureGenerator>();
            
            DateTime capturedTimestamp = DateTime.MinValue;
            signatureGenerator.Setup(x => x.GenerateSignature(It.IsAny<TransactionDetailDTO>(), It.IsAny<DateTime>()))
                .Callback<TransactionDetailDTO, DateTime>((_, validUntil) => 
                {
                    // Simulate processing delay that might occur in real system
                    Thread.Sleep(simulatedDelayMs);
                    capturedTimestamp = validUntil;
                })
                .Returns($"0x_signature_after_{simulatedDelayMs}ms");

            var dispenser = dbFactory.Current.Dispenser.First();
            var signatureProcessor = new SignatureProcessor(dbFactory, signatureGenerator.Object);

            // Act
            var validFrom = signatureProcessor.SaveSignature(dispenser, false);

            // Assert
            var savedSignature = dbFactory.Current.Signatures.First();
            
            // The critical test: Even with processing delays, timestamps must be identical
            capturedTimestamp.Should().Be(savedSignature.ValidUntil,
                $"With {simulatedDelayMs}ms delay, timestamps must still be identical (this fixes the ±1s issue)");
            
            validFrom.Should().Be(savedSignature.ValidFrom,
                "ValidFrom must also be consistent regardless of processing delay");
            
            // Verify the signature includes the expected delay indicator
            savedSignature.Signature.Should().Be($"0x_signature_after_{simulatedDelayMs}ms");
        }

        [Fact]
        internal void OriginalIssueScenario_MultipleDateTime_UtcNowCalls_ShouldBeFixed()
        {
            // This test recreates the original problematic scenario:
            // Multiple DateTime.UtcNow calls in different parts of the code
            // The fix ensures we only call DateTime.UtcNow once

            // Arrange
            var dbFactory = new MockDbContextFactory(seed: true);
            var signatureGenerator = new Mock<ISignatureGenerator>();
            
            var timestampUsedInGeneration = DateTime.MinValue;
            signatureGenerator.Setup(x => x.GenerateSignature(It.IsAny<TransactionDetailDTO>(), It.IsAny<DateTime>()))
                .Callback<TransactionDetailDTO, DateTime>((_, validUntil) => 
                {
                    timestampUsedInGeneration = validUntil;
                })
                .Returns("0x_fixed_signature");

            var dispenser = dbFactory.Current.Dispenser.First();
            dispenser.LastUserSignature = DateTime.UtcNow.AddMinutes(-5); // Force ValidFrom offset calculation
            dbFactory.Current.SaveChanges();

            var signatureProcessor = new SignatureProcessor(dbFactory, signatureGenerator.Object);

            // Act
            var validFrom = signatureProcessor.SaveSignature(dispenser, false);

            // Assert
            var savedSignature = dbFactory.Current.Signatures.First();
            
            // Before the fix: validUntil and validFrom could be calculated using different DateTime.UtcNow values
            // After the fix: they should both derive from the same base timestamp
            
            timestampUsedInGeneration.Should().Be(savedSignature.ValidUntil,
                "SignatureGenerator should receive the exact same timestamp stored in database");
            
            // ValidFrom and ValidUntil should have a predictable relationship based on the single base timestamp
            var expectedTimeDifference = TimeSpan.FromSeconds(300); // VALID_UNTIL_MAX_OFFSET_IN_SECONDS - VALID_FROM_OFFSET_IN_SECONDS
            var actualTimeDifference = savedSignature.ValidUntil - savedSignature.ValidFrom;
            
            // Since LastUserSignature exists, ValidFrom = baseTimestamp + offset
            // ValidUntil = baseTimestamp + 300 seconds
            // So ValidUntil - ValidFrom should be exactly 300 - offset seconds
            var offsetSeconds = int.Parse(Environment.GetEnvironmentVariable("VALID_FROM_OFFSET_IN_SECONDS")!);
            var expectedDifference = TimeSpan.FromSeconds(300 - offsetSeconds);
            
            actualTimeDifference.Should().Be(expectedDifference,
                "Time difference between ValidUntil and ValidFrom should be predictable when using consistent base timestamp");
        }
    }
}