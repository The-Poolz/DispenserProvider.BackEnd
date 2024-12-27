using Moq;
using Xunit;
using SecretsManager;
using FluentAssertions;
using EnvironmentManager.Extensions;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Services.Handlers.GenerateSignature.Helpers;

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
            var secretManager = new Mock<SecretManager>();

            secretManager.Setup(x => x.GetSecretValue(Env.SECRET_ID_OF_SIGN_ACCOUNT.GetRequired<string>(null), Env.SECRET_KEY_OF_SIGN_ACCOUNT.GetRequired<string>(null)))
                .Returns(PrivateKey);

            var transactionDetail = MockDispenserContext.TransactionDetail;
            transactionDetail.WithdrawalDispenser = MockDispenserContext.Dispenser;
            var validUntil = DateTime.UtcNow.AddMinutes(10);

            var signatureGenerator = new SignatureGenerator(secretManager.Object);

            var signature = signatureGenerator.GenerateSignature(transactionDetail, validUntil);

            signature.Should().NotBeNullOrWhiteSpace();
        }
    }
}