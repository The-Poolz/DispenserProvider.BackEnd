using Moq;
using Xunit;
using FluentAssertions;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;
using Nethereum.Util;

namespace DispenserProvider.Tests.Services.Handlers.GenerateSignature.Web3;

/// <summary>
/// Integration test that validates the complete fix for issue #146
/// This test specifically recreates the problematic scenario described in the issue
/// </summary>
public class Issue146IntegrationTest
{
    public class SignatureTimestampConsistencyFix
    {
        public SignatureTimestampConsistencyFix()
        {
            Environment.SetEnvironmentVariable("VALID_UNTIL_MAX_OFFSET_IN_SECONDS", "300");
            Environment.SetEnvironmentVariable("VALID_FROM_OFFSET_IN_SECONDS", "300");
        }

        [Fact]
        internal void Issue146_OriginalProblem_ShouldBeResolved()
        {
            // Issue Description: 
            // "Signature timestamp is generated separately in different modules ("fresh" time at each call).
            // If >500 ms elapse between those calls, one side may round the timestamp up by +1 s 
            // while the other doesn't, causing a signature mismatch."

            // Arrange
            var dbFactory = new MockDbContextFactory(seed: true);
            var signatureGenerator = new Mock<ISignatureGenerator>();
            
            // Track exactly what timestamp the signature generator receives
            DateTime timestampUsedForSignature = DateTime.MinValue;
            signatureGenerator.Setup(x => x.GenerateSignature(It.IsAny<TransactionDetailDTO>(), It.IsAny<DateTime>()))
                .Callback<TransactionDetailDTO, DateTime>((_, validUntil) => 
                {
                    timestampUsedForSignature = validUntil;
                    
                    // Simulate the signature generation process that caused the original issue
                    // In the real system, this would involve EIP712 signing which uses Unix timestamps
                    var unixTimestamp = validUntil.ToUnixTimestamp();
                    
                    // The signature should be deterministic based on the Unix timestamp
                    return;
                })
                .Returns("0x_deterministic_signature_based_on_timestamp");

            var dispenser = dbFactory.Current.Dispenser.First();
            var signatureProcessor = new SignatureProcessor(dbFactory, signatureGenerator.Object);

            // Act - This would previously cause timestamp inconsistency
            var validFromResult = signatureProcessor.SaveSignature(dispenser, false);

            // Assert - The fix ensures timestamp consistency
            var savedSignature = dbFactory.Current.Signatures.First();
            
            // CRITICAL: The timestamp used for signature generation must be IDENTICAL 
            // to the timestamp stored in the database
            timestampUsedForSignature.Should().Be(savedSignature.ValidUntil,
                "The signature generator must receive the exact same timestamp that gets stored in the database. " +
                "This prevents the ±1 second rounding issue that caused signature validation failures.");

            // Verify that ValidFrom is also calculated from the same base timestamp
            validFromResult.Should().Be(savedSignature.ValidFrom,
                "ValidFrom must be consistently calculated from the same base timestamp");

            // Additional validation: Unix timestamp conversion should be consistent
            var storedUnixTimestamp = savedSignature.ValidUntil.ToUnixTimestamp();
            var generatorUnixTimestamp = timestampUsedForSignature.ToUnixTimestamp();
            
            storedUnixTimestamp.Should().Be(generatorUnixTimestamp,
                "Unix timestamp conversion must be identical, preventing signature validation mismatches");
        }

        [Fact]
        internal void Issue146_MultipleCallsScenario_ShouldNotCauseInconsistency()
        {
            // This test validates that the fix prevents the scenario where:
            // "one side may round the timestamp up by +1 s while the other doesn't"

            // Arrange
            var dbFactory = new MockDbContextFactory(seed: true);
            var signatureGenerator = new Mock<ISignatureGenerator>();
            
            var allTimestamps = new List<(DateTime validUntil, long unixTimestamp)>();
            signatureGenerator.Setup(x => x.GenerateSignature(It.IsAny<TransactionDetailDTO>(), It.IsAny<DateTime>()))
                .Callback<TransactionDetailDTO, DateTime>((_, validUntil) => 
                {
                    var unixTimestamp = validUntil.ToUnixTimestamp();
                    allTimestamps.Add((validUntil, unixTimestamp));
                })
                .Returns("0x_signature");

            var dispenser = dbFactory.Current.Dispenser.First();
            var signatureProcessor = new SignatureProcessor(dbFactory, signatureGenerator.Object);

            // Act - Generate multiple signatures to test consistency
            var results = new List<DateTime>();
            for (int i = 0; i < 5; i++)
            {
                results.Add(signatureProcessor.SaveSignature(dispenser, false));
                
                // Introduce delays that could cause the original timestamp rounding issue
                Thread.Sleep(150); // Total: 750ms across all iterations
            }

            // Assert
            var savedSignatures = dbFactory.Current.Signatures.OrderBy(x => x.Id).ToList();
            savedSignatures.Should().HaveCount(5);
            allTimestamps.Should().HaveCount(5);

            // Verify that each signature operation maintained timestamp consistency
            for (int i = 0; i < 5; i++)
            {
                var (validUntil, unixTimestamp) = allTimestamps[i];
                var savedSignature = savedSignatures[i];
                
                // The core fix validation
                validUntil.Should().Be(savedSignature.ValidUntil,
                    $"Signature {i}: Generator timestamp must match database timestamp");
                
                unixTimestamp.Should().Be(savedSignature.ValidUntil.ToUnixTimestamp(),
                    $"Signature {i}: Unix timestamp must be consistent");
            }
        }

        [Theory]
        [InlineData(999)]  // Just under 1 second - the critical boundary
        [InlineData(1000)] // Exactly 1 second
        [InlineData(1001)] // Just over 1 second
        internal void Issue146_SecondBoundaryScenario_ShouldMaintainConsistency(int delayMs)
        {
            // This test specifically targets the ±1 second rounding issue

            // Arrange
            var dbFactory = new MockDbContextFactory(seed: true);
            var signatureGenerator = new Mock<ISignatureGenerator>();
            
            DateTime generatorTimestamp = DateTime.MinValue;
            long generatorUnixTimestamp = 0;
            
            signatureGenerator.Setup(x => x.GenerateSignature(It.IsAny<TransactionDetailDTO>(), It.IsAny<DateTime>()))
                .Callback<TransactionDetailDTO, DateTime>((_, validUntil) => 
                {
                    // Simulate processing delay that could cause rounding issues
                    Thread.Sleep(delayMs);
                    
                    generatorTimestamp = validUntil;
                    generatorUnixTimestamp = validUntil.ToUnixTimestamp();
                })
                .Returns($"0x_signature_with_{delayMs}ms_delay");

            var dispenser = dbFactory.Current.Dispenser.First();
            var signatureProcessor = new SignatureProcessor(dbFactory, signatureGenerator.Object);

            // Act
            var validFrom = signatureProcessor.SaveSignature(dispenser, false);

            // Assert
            var savedSignature = dbFactory.Current.Signatures.First();
            
            // The key validation: Even with delays, timestamps must be identical
            generatorTimestamp.Should().Be(savedSignature.ValidUntil,
                $"With {delayMs}ms delay, timestamp consistency must be maintained");
            
            generatorUnixTimestamp.Should().Be(savedSignature.ValidUntil.ToUnixTimestamp(),
                $"With {delayMs}ms delay, Unix timestamp must be consistent");
            
            // Verify the signature reflects the expected processing
            savedSignature.Signature.Should().Be($"0x_signature_with_{delayMs}ms_delay");
        }
    }
}