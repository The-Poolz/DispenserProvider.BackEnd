using Moq;
using Xunit;
using FluentAssertions;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;
using Nethereum.Util;

namespace DispenserProvider.Tests.Services.Handlers.GenerateSignature.Web3;

/// <summary>
/// Tests for edge cases related to timestamp parsing discrepancies (±1s) as requested in issue #146
/// </summary>
public class SignatureTimestampEdgeCaseTests
{
    public class TimestampParsingDiscrepancies
    {
        public TimestampParsingDiscrepancies()
        {
            Environment.SetEnvironmentVariable("VALID_UNTIL_MAX_OFFSET_IN_SECONDS", "300");
            Environment.SetEnvironmentVariable("VALID_FROM_OFFSET_IN_SECONDS", "300");
        }

        [Fact]
        internal void WhenSignatureGeneratedMultipleTimes_ShouldHaveConsistentUnixTimestamps()
        {
            // Arrange
            var dbFactory = new MockDbContextFactory(seed: true);
            var signatureGenerator = new Mock<ISignatureGenerator>();
            
            var capturedValidUntilTimestamps = new List<DateTime>();
            signatureGenerator.Setup(x => x.GenerateSignature(It.IsAny<TransactionDetailDTO>(), It.IsAny<DateTime>()))
                .Callback<TransactionDetailDTO, DateTime>((_, validUntil) => 
                {
                    capturedValidUntilTimestamps.Add(validUntil);
                })
                .Returns("0x");

            var dispenser = dbFactory.Current.Dispenser.First();
            var signatureProcessor = new SignatureProcessor(dbFactory, signatureGenerator.Object);

            // Act - Generate signatures in quick succession to test timestamp consistency
            var results = new List<DateTime>();
            for (int i = 0; i < 5; i++)
            {
                results.Add(signatureProcessor.SaveSignature(dispenser, false));
                // Small delay to potentially trigger timestamp boundaries
                Thread.Sleep(10);
            }

            // Assert
            capturedValidUntilTimestamps.Should().HaveCount(5);
            var signatures = dbFactory.Current.Signatures.OrderBy(x => x.Id).ToList();
            signatures.Should().HaveCount(5);

            // Verify that each captured timestamp matches the stored one exactly
            for (int i = 0; i < 5; i++)
            {
                capturedValidUntilTimestamps[i].Should().Be(signatures[i].ValidUntil,
                    $"Captured ValidUntil timestamp {i} should match stored timestamp");
                
                results[i].Should().Be(signatures[i].ValidFrom,
                    $"Returned ValidFrom timestamp {i} should match stored timestamp");
            }
        }

        [Fact]
        internal void WhenProcessingNearUnixTimestampBoundary_ShouldGenerateConsistentSignature()
        {
            // Arrange
            var dbFactory = new MockDbContextFactory(seed: true);
            var signatureGenerator = new Mock<ISignatureGenerator>();
            
            DateTime capturedValidUntil = DateTime.MinValue;
            string capturedSignature = string.Empty;
            
            signatureGenerator.Setup(x => x.GenerateSignature(It.IsAny<TransactionDetailDTO>(), It.IsAny<DateTime>()))
                .Callback<TransactionDetailDTO, DateTime>((_, validUntil) => 
                {
                    capturedValidUntil = validUntil;
                    // Verify that the Unix timestamp conversion is consistent
                    var unixTimestamp = validUntil.ToUnixTimestamp();
                    capturedSignature = $"signature_for_timestamp_{unixTimestamp}";
                })
                .Returns(() => capturedSignature);

            var dispenser = dbFactory.Current.Dispenser.First();
            var signatureProcessor = new SignatureProcessor(dbFactory, signatureGenerator.Object);

            // Act
            var validFrom = signatureProcessor.SaveSignature(dispenser, false);

            // Assert
            var savedSignature = dbFactory.Current.Signatures.First();
            
            // The captured timestamp should exactly match the stored one
            capturedValidUntil.Should().Be(savedSignature.ValidUntil);
            
            // The Unix timestamp should be consistent when converted
            var storedUnixTimestamp = savedSignature.ValidUntil.ToUnixTimestamp();
            var expectedSignature = $"signature_for_timestamp_{storedUnixTimestamp}";
            
            savedSignature.Signature.Should().Be(expectedSignature,
                "Signature should be generated using consistent Unix timestamp");
        }

        [Theory]
        [InlineData(true)]  // Test refund scenario
        [InlineData(false)] // Test withdrawal scenario
        internal void WhenCalculatingTimestampsForRefundVsWithdrawal_ShouldUseConsistentBaseTime(bool isRefund)
        {
            // Arrange
            var dbFactory = new MockDbContextFactory(seed: true);
            var signatureGenerator = new Mock<ISignatureGenerator>();
            
            DateTime capturedValidUntil = DateTime.MinValue;
            signatureGenerator.Setup(x => x.GenerateSignature(It.IsAny<TransactionDetailDTO>(), It.IsAny<DateTime>()))
                .Callback<TransactionDetailDTO, DateTime>((_, validUntil) => 
                {
                    capturedValidUntil = validUntil;
                })
                .Returns("0x");

            var dispenser = dbFactory.Current.Dispenser.First();
            
            // Setup refund detail if needed
            if (isRefund)
            {
                var refundDetail = new TransactionDetailDTO
                {
                    Id = 2,
                    UserAddress = "0x0000000000000000000000000000000000000001",
                    ChainId = 56,
                    PoolId = 1,
                    RefundDispenser = dispenser
                };
                dbFactory.Current.Add(refundDetail);
                dbFactory.Current.SaveChanges();
                
                // Reload dispenser to get updated data
                dispenser = dbFactory.Current.Dispenser.First();
            }

            var signatureProcessor = new SignatureProcessor(dbFactory, signatureGenerator.Object);

            // Act
            var validFrom = signatureProcessor.SaveSignature(dispenser, isRefund);

            // Assert
            var savedSignature = dbFactory.Current.Signatures.First();
            
            // Both ValidFrom and ValidUntil should be calculated from the same base timestamp
            capturedValidUntil.Should().Be(savedSignature.ValidUntil);
            validFrom.Should().Be(savedSignature.ValidFrom);

            // ValidUntil should always be after ValidFrom (unless constrained by refund finish time)
            if (!isRefund || savedSignature.ValidUntil < dispenser.RefundFinishTime)
            {
                savedSignature.ValidUntil.Should().BeAfter(savedSignature.ValidFrom);
            }
        }

        [Fact]
        internal void WhenDispenserHasLastUserSignature_ShouldCalculateValidFromWithConsistentOffset()
        {
            // Arrange
            var dbFactory = new MockDbContextFactory(seed: true);
            var signatureGenerator = new Mock<ISignatureGenerator>();
            
            DateTime capturedValidUntil = DateTime.MinValue;
            signatureGenerator.Setup(x => x.GenerateSignature(It.IsAny<TransactionDetailDTO>(), It.IsAny<DateTime>()))
                .Callback<TransactionDetailDTO, DateTime>((_, validUntil) => 
                {
                    capturedValidUntil = validUntil;
                })
                .Returns("0x");

            var dispenser = dbFactory.Current.Dispenser.First();
            dispenser.LastUserSignature = DateTime.UtcNow.AddHours(-1);
            dbFactory.Current.SaveChanges();

            var signatureProcessor = new SignatureProcessor(dbFactory, signatureGenerator.Object);

            // Act
            var validFrom = signatureProcessor.SaveSignature(dispenser, false);

            // Assert
            var savedSignature = dbFactory.Current.Signatures.First();
            var offsetSeconds = Environment.GetEnvironmentVariable("VALID_FROM_OFFSET_IN_SECONDS");
            var expectedOffset = TimeSpan.FromSeconds(int.Parse(offsetSeconds!));
            
            // ValidFrom should be ValidUntil minus the expected duration plus offset
            // Since both are calculated from the same base timestamp
            var calculatedValidFrom = capturedValidUntil - TimeSpan.FromSeconds(300) + expectedOffset;
            
            savedSignature.ValidFrom.Should().BeCloseTo(calculatedValidFrom, TimeSpan.FromMilliseconds(1),
                "ValidFrom should be calculated consistently with ValidUntil using the same base timestamp");
        }
    }
}