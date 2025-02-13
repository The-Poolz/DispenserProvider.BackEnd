using Xunit;
using FluentAssertions;
using DispenserProvider.Services.Web3;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;
using DispenserProvider.Tests.Mocks.Services.Handlers.GenerateSignature.Web3;

namespace DispenserProvider.Tests.Services.Handlers.GenerateSignature.Web3;

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
            var signerManager = new MockSignerManager(PrivateKey);
            var chainProvider = new ChainProvider(MockCovalentContext.Create());
            var signatureGenerator = new SignatureGenerator(signerManager, chainProvider);

            var transactionDetail = MockDispenserContext.TransactionDetail;
            transactionDetail.Builders = [MockDispenserContext.Builder];
            transactionDetail.WithdrawalDispenser = MockDispenserContext.Dispenser;

            var signature = signatureGenerator.GenerateSignature(transactionDetail, DateTime.UtcNow.AddMinutes(10));

            signature.Should().NotBeNullOrWhiteSpace();
        }
    }
}