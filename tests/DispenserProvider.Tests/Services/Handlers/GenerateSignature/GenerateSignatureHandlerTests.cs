using Moq;
using Xunit;
using FluentValidation;
using FluentAssertions;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Database;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Services.Handlers.GenerateSignature;
using DispenserProvider.Services.Validators.Signature.Models;
using DispenserProvider.Services.Handlers.GenerateSignature.Models;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;

namespace DispenserProvider.Tests.Services.Handlers.GenerateSignature;

public class GenerateSignatureHandlerTests
{
    public class Handle
    {
        private readonly GenerateSignatureRequest _withdrawnRequest = new()
        {
            PoolId = 1,
            ChainId = 1,
            UserAddress = "0x0000000000000000000000000000000000000001"
        };

        private readonly GenerateSignatureRequest _refundRequest = new()
        {
            PoolId = 1,
            ChainId = 56,
            UserAddress = "0x0000000000000000000000000000000000000001"
        };

        [Fact]
        internal void WhenDispenserNotFound_ShouldThrowException()
        {
            var handler = new GenerateSignatureHandler(
                new DispenserManager(new MockDbContextFactory(seed: false)),
                new Mock<ISignatureProcessor>().Object,
                new Mock<IValidator<GenerateSignatureValidatorRequest>>().Object
            );

            var testCode = () => handler.Handle(_withdrawnRequest);

            testCode.Should().Throw<InvalidOperationException>()
                .WithMessage($"*Asset by provided PoolId={_withdrawnRequest.PoolId} and ChainId={_withdrawnRequest.ChainId} for '{_withdrawnRequest.UserAddress}' user, not found.*");
        }

        [Fact]
        internal void WhenDispenserFoundAndIsNotRefund_ShouldReturnExpectedResult()
        {
            var handler = new GenerateSignatureHandler(
                new DispenserManager(new MockDbContextFactory(seed: true)),
                new Mock<ISignatureProcessor>().Object,
                new Mock<IValidator<GenerateSignatureValidatorRequest>>().Object
            );

            var response = handler.Handle(_withdrawnRequest);

            response.Should().NotBeNull();
        }

        [Fact]
        internal void When_DispenserFound_AndIsRefund_ShouldReturnExpectedResult()
        {
            var dispenserContextFactory = new MockDbContextFactory(seed: true);
            var refundDetail = new TransactionDetailDTO
            {
                Id = 2,
                ChainId = _refundRequest.ChainId,
                PoolId = _refundRequest.PoolId,
                RefundDispenser = dispenserContextFactory.Current.Dispenser.First()
            };
            dispenserContextFactory.Current.Add(refundDetail);
            dispenserContextFactory.Current.SaveChanges();

            var handler = new GenerateSignatureHandler(
                new DispenserManager(dispenserContextFactory),
                new Mock<ISignatureProcessor>().Object,
                new Mock<IValidator<GenerateSignatureValidatorRequest>>().Object
            );

            var response = handler.Handle(_refundRequest);

            response.Should().NotBeNull();
        }
    }
}