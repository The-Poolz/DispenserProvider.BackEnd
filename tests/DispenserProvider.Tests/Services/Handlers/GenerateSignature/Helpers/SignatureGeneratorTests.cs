using Moq;
using Xunit;
using FluentAssertions;
using Nethereum.Signer;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;
using DispenserProvider.Services.Handlers.GenerateSignature.Helpers;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;

namespace DispenserProvider.Tests.Services.Handlers.GenerateSignature.Helpers;

public class SignatureGeneratorTests
{
    public class GenerateSignature
    {
        private const string PrivateKey = "0xadb0a6fc56e496c08ebbe27515288d333ecb9cf714442129c0bd1a7952416789";

        public GenerateSignature()
        {
            Environment.SetEnvironmentVariable("SECRET_ID_OF_SIGN_ACCOUNT", "dispenser");
            Environment.SetEnvironmentVariable("SECRET_KEY_OF_SIGN_ACCOUNT", "privateKey");
        }

        [Fact]
        internal void WhenGeneratedSuccessfully_ShouldReturnSignature()
        {
            var signerManager = new Mock<ISignerManager>();
            signerManager.Setup(x => x.GetSigner())
                .Returns(new EthECKey(PrivateKey));

            var transactionDetail = MockDispenserContext.TransactionDetail;
            transactionDetail.WithdrawalDispenser = MockDispenserContext.Dispenser;
            var validUntil = DateTime.UtcNow.AddMinutes(10);

            var signatureGenerator = new SignatureGenerator(signerManager.Object);

            var signature = signatureGenerator.GenerateSignature(transactionDetail, validUntil);

            signature.Should().NotBeNullOrWhiteSpace();
        }
    }
}