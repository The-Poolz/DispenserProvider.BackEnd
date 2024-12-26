using Xunit;
using FluentAssertions;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Services.Handlers.ReadAsset;
using DispenserProvider.Services.Handlers.ReadAsset.Models;

namespace DispenserProvider.Tests.Services.Handlers.ReadAsset;

public class ReadAssetHandlerTests
{
    public class Handle
    {
        private readonly ReadAssetHandler handler = new(MockDispenserContext.Create(seed: true));

        [Fact]
        internal void WhenAssetFound_ShouldReturnsExpectedAsset()
        {
            var request = new ReadAssetRequest {
                AssetContext = [
                    new AssetContext {
                        PoolId = 1,
                        ChainId = 1
                    }
                ]
            };

            var response = handler.Handle(request);

            response.Assets.Should().HaveCount(1)
                .And.ContainSingle(asset =>
                    asset.ChainId == request.AssetContext[0].ChainId &&
                    asset.PoolId == request.AssetContext[0].PoolId
                );

            response.Assets[0].Dispensers.Should().HaveCount(1)
                .And.ContainSingle(dispenser =>
                    dispenser.UserAddress == MockDispenserContext.Dispenser.UserAddress &&
                    dispenser.RefundFinishTime == MockDispenserContext.Dispenser.RefundFinishTime
                );

            response.Assets[0].Dispensers[0].Builders.Should().HaveCount(1)
                .And.ContainSingle(builder =>
                    builder.ProviderAddress == MockDispenserContext.Builder.ProviderAddress &&
                    builder.WeiAmount == MockDispenserContext.Builder.WeiAmount &&
                    builder.StartTime == MockDispenserContext.Builder.StartTime &&
                    builder.FinishTime == MockDispenserContext.Builder.FinishTime
                );
        }

        [Fact]
        internal void WhenAssetNotFound_ShouldReturnsEmptyArray()
        {
            var request = new ReadAssetRequest {
                AssetContext = [
                    new AssetContext {
                        PoolId = 123,
                        ChainId = 123
                    }
                ]
            };

            var response = handler.Handle(request);

            response.Assets.Should().HaveCount(1)
                .And.ContainSingle(asset =>
                    asset.ChainId == request.AssetContext[0].ChainId &&
                    asset.PoolId == request.AssetContext[0].PoolId
                );

            response.Assets[0].Dispensers.Should().HaveCount(0);
        }
    }
}