using Moq;
using Xunit;
using FluentAssertions;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Database;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Services.Handlers.GenerateSignature;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;
using DispenserProvider.Services.Handlers.GenerateSignature.Models;

namespace DispenserProvider.Tests.Services.Handlers.GenerateSignature;

public class GenerateSignatureHandlerTests
{
    public class Handle
    {
        private readonly GenerateSignatureRequest _withdrawnRequest = new(97, 1, "0x0000000000000000000000000000000000000001");
        private readonly GenerateSignatureRequest _refundRequest = new(56, 1, "0x0000000000000000000000000000000000000001");

        [Fact]
        internal void WhenDispenserFoundAndIsNotRefund_ShouldReturnExpectedResult()
        {
            var dispenserContextFactory = new MockDbContextFactory(seed: true);
            var dispenser = dispenserContextFactory.Current.Dispenser.First();
            var handler = new GenerateSignatureHandler(
                new DispenserManager(dispenserContextFactory),
                new Mock<ISignatureProcessor>().Object
            );

            var response = handler.Handle(_withdrawnRequest, CancellationToken.None);

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
                new Mock<ISignatureProcessor>().Object
            );

            var response = handler.Handle(_refundRequest, CancellationToken.None);

            response.Should().NotBeNull();
        }
    }
}