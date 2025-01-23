using Xunit;
using FluentAssertions;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Services.Handlers.ListOfAssets;
using DispenserProvider.Services.Handlers.ListOfAssets.Models;

namespace DispenserProvider.Tests.Services.Handlers.ListOfAssets;

public class ListOfAssetsHandlerTests
{
    public class Handle
    {
        private readonly ListOfAssetsHandler _handler = new(new MockDbContextFactory(seed: true));

        [Fact]
        internal void WhenAssetsFound_ShouldReturnsExpectedAssets()
        {
            var request = new ListOfAssetsRequest {
                UserAddress = MockDispenserContext.Dispenser.UserAddress
            };

            var response = _handler.Handle(request);

            response.Assets.Should().HaveCount(1)
                .And.ContainSingle(x =>
                    x.UserAddress == MockDispenserContext.Dispenser.UserAddress &&
                    x.RefundFinishTime == MockDispenserContext.Dispenser.RefundFinishTime &&
                    x.WithdrawalDetail.PoolId == MockDispenserContext.TransactionDetail.PoolId &&
                    x.WithdrawalDetail.ChainId == MockDispenserContext.TransactionDetail.ChainId &&
                    x.WithdrawalDetail.Builders[0].WeiAmount == MockDispenserContext.Builder.WeiAmount &&
                    x.WithdrawalDetail.Builders[0].ProviderAddress == MockDispenserContext.Builder.ProviderAddress &&
                    x.WithdrawalDetail.Builders[0].StartTime == MockDispenserContext.Builder.StartTime &&
                    x.WithdrawalDetail.Builders[0].FinishTime == MockDispenserContext.Builder.FinishTime &&
                    x.RefundDetail == null
                );
        }

        [Fact]
        internal void WhenAssetNotFound_ShouldReturnsEmptyArray()
        {
            var request = new ListOfAssetsRequest {
                UserAddress = "0x0000000000000000000000000000000000000101"
            };

            var response = _handler.Handle(request);

            response.Assets.Should().BeEmpty();
        }
    }
}