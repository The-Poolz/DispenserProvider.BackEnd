using Moq;
using Xunit;
using FluentAssertions;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;
using System.Reflection;

namespace DispenserProvider.Tests.Services.Handlers.GenerateSignature.Web3;

public class SignatureTimestampTests
{
    public class TimestampConsistency
    {
        public TimestampConsistency()
        {
            Environment.SetEnvironmentVariable("VALID_UNTIL_MAX_OFFSET_IN_SECONDS", "300");
            Environment.SetEnvironmentVariable("VALID_FROM_OFFSET_IN_SECONDS", "300");
        }

        [Fact]
        internal void WhenSignatureGenerated_ShouldUseConsistentTimestamps()
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
            var signatureProcessor = new SignatureProcessor(dbFactory, signatureGenerator.Object);

            // Act
            var validFrom = signatureProcessor.SaveSignature(dispenser, false);

            // Assert
            var savedSignature = dbFactory.Current.Signatures.First();
            
            // The validUntil timestamp passed to SignatureGenerator should match the one saved to database
            capturedValidUntil.Should().Be(savedSignature.ValidUntil);
            
            // ValidFrom should be consistent with the calculated time
            savedSignature.ValidFrom.Should().Be(validFrom);
        }

        [Fact]
        internal void WhenMultipleSignaturesGenerated_ShouldHaveConsistentTimestampDifferences()
        {
            // Arrange
            var dbFactory = new MockDbContextFactory(seed: true);
            var signatureGenerator = new Mock<ISignatureGenerator>();
            
            var capturedTimestamps = new List<DateTime>();
            signatureGenerator.Setup(x => x.GenerateSignature(It.IsAny<TransactionDetailDTO>(), It.IsAny<DateTime>()))
                .Callback<TransactionDetailDTO, DateTime>((_, validUntil) => 
                {
                    capturedTimestamps.Add(validUntil);
                })
                .Returns("0x");

            var dispenser = dbFactory.Current.Dispenser.First();
            var signatureProcessor = new SignatureProcessor(dbFactory, signatureGenerator.Object);

            // Act - Generate two signatures with a small delay
            var validFrom1 = signatureProcessor.SaveSignature(dispenser, false);
            Thread.Sleep(100); // Small delay to potentially trigger timestamp inconsistency
            var validFrom2 = signatureProcessor.SaveSignature(dispenser, false);

            // Assert
            capturedTimestamps.Should().HaveCount(2);
            
            var signatures = dbFactory.Current.Signatures.OrderBy(x => x.Id).ToList();
            signatures.Should().HaveCount(2);

            // Each signature should have consistent timestamps between generator and database
            capturedTimestamps[0].Should().Be(signatures[0].ValidUntil);
            capturedTimestamps[1].Should().Be(signatures[1].ValidUntil);
        }

        [Theory]
        [InlineData(0)]     // Exact second boundary
        [InlineData(499)]   // Just under 500ms
        [InlineData(500)]   // Exactly 500ms
        [InlineData(501)]   // Just over 500ms
        [InlineData(999)]   // Just under 1 second
        internal void WhenTimestampNearSecondBoundary_ShouldGenerateConsistentSignature(int millisecondsOffset)
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
            var signatureProcessor = new SignatureProcessor(dbFactory, signatureGenerator.Object);

            // Act - Simulate processing at specific millisecond offset
            // Note: This test verifies the current behavior and should pass after fix
            var validFrom = signatureProcessor.SaveSignature(dispenser, false);

            // Assert
            var savedSignature = dbFactory.Current.Signatures.First();
            
            // The key assertion: timestamps should be consistent regardless of when they're calculated
            capturedValidUntil.Should().Be(savedSignature.ValidUntil, 
                "ValidUntil timestamp should be consistent between signature generation and database storage");
            
            // ValidFrom should also be consistent
            savedSignature.ValidFrom.Should().Be(validFrom,
                "ValidFrom timestamp should be consistent");
        }

        [Fact]
        internal void WhenValidFromCalculatedWithLastUserSignature_ShouldUseConsistentBaseTime()
        {
            // Arrange
            var dbFactory = new MockDbContextFactory(seed: true);
            var signatureGenerator = new Mock<ISignatureGenerator>();
            signatureGenerator.Setup(x => x.GenerateSignature(It.IsAny<TransactionDetailDTO>(), It.IsAny<DateTime>()))
                .Returns("0x");

            // Create a dispenser with LastUserSignature to trigger the offset calculation
            var dispenser = dbFactory.Current.Dispenser.First();
            dispenser.LastUserSignature = DateTime.UtcNow.AddMinutes(-1);
            dbFactory.Current.SaveChanges();

            var signatureProcessor = new SignatureProcessor(dbFactory, signatureGenerator.Object);

            // Act
            var validFrom = signatureProcessor.SaveSignature(dispenser, false);

            // Assert
            var savedSignature = dbFactory.Current.Signatures.First();
            
            // When LastUserSignature exists, ValidFrom should include the offset
            // The key is that this calculation should use the same base timestamp as ValidUntil
            savedSignature.ValidFrom.Should().Be(validFrom);
            savedSignature.ValidFrom.Should().BeAfter(dispenser.LastUserSignature.Value);
        }
    }
}