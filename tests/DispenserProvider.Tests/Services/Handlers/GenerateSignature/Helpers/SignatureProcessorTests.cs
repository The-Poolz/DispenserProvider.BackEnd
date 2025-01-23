using Moq;
using Xunit;
using FluentAssertions;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Services.Handlers.GenerateSignature.Helpers;

namespace DispenserProvider.Tests.Services.Handlers.GenerateSignature.Helpers;

public class SignatureProcessorTests
{
    public class SaveSignature
    {
        public SaveSignature()
        {
            Environment.SetEnvironmentVariable("VALID_UNTIL_MAX_OFFSET_IN_SECONDS", "300");
            Environment.SetEnvironmentVariable("VALID_FROM_OFFSET_IN_SECONDS", "300");
        }

        [Fact]
        internal void WhenSavedSuccessfully()
        {
            const string signature = "0x";
            var dbFactory = new MockDbContextFactory();
            var signatureGenerator = new Mock<ISignatureGenerator>();
            signatureGenerator.Setup(x => x.GenerateSignature(It.IsAny<TransactionDetailDTO>(), It.IsAny<DateTime>()))
                .Returns(signature);

            const bool isRefund = false;
            var dispenser = dbFactory.Current.Dispenser.First();

            var signatureProcessor = new SignatureProcessor(dbFactory, signatureGenerator.Object);

            var response = signatureProcessor.SaveSignature(dispenser, isRefund);

            dbFactory.Current.Signatures.ToArray().Should().HaveCount(1)
                .And.ContainSingle(x =>
                    x.IsRefund == isRefund &&
                    x.DispenserId == MockDispenserContext.Dispenser.Id &&
                    x.Signature == signature &&
                    x.ValidFrom == response
                );
        }
    }
}