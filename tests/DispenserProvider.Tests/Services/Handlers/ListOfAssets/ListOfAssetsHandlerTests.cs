using Xunit;
using FluentAssertions;
using DispenserProvider.Services.Database;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Tests.Mocks.Services.Web3;
using DispenserProvider.Services.Validators.Signature;
using DispenserProvider.Services.Handlers.ListOfAssets;
using DispenserProvider.Services.Handlers.ListOfAssets.Models;

namespace DispenserProvider.Tests.Services.Handlers.ListOfAssets;

public class ListOfAssetsHandlerTests
{
    public class Handle
    {
        [Fact]
        internal async Task WhenAssetsFound_ShouldReturnsExpectedAssets()
        {
            var dbFactory = new MockDbContextFactory(seed: true);
            var dispenser = dbFactory.Current.Dispenser.First();
            var dispenserContract = MockDispenserProviderContract.Create(dispenser, isWithdrawn: false, isRefunded: false);
            var takenTrackManager = new TakenTrackManager(dbFactory, new AssetAvailabilityValidator(dispenserContract));
            var handler = new ListOfAssetsHandler(dbFactory, takenTrackManager);

            var request = new ListOfAssetsRequest
            {
                UserAddress = MockDispenserContext.Dispenser.UserAddress
            };

            var response = await handler.Handle(request, CancellationToken.None);

            response.Count.Should().Be(1);
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
            dbFactory.Current.TakenTrack.ToArray().Should().BeEmpty();
        }

        [Fact]
        internal async Task WhenAssetIsTracked_ShouldReturnsEmptyCollection()
        {
            var dbFactory = new MockDbContextFactory(seed: true);
            var dispenser = dbFactory.Current.Dispenser.First();
            var dispenserContract = MockDispenserProviderContract.Create(dispenser, isWithdrawn: true, isRefunded: false);
            var takenTrackManager = new TakenTrackManager(dbFactory, new AssetAvailabilityValidator(dispenserContract));
            var handler = new ListOfAssetsHandler(dbFactory, takenTrackManager);

            var request = new ListOfAssetsRequest
            {
                UserAddress = MockDispenserContext.Dispenser.UserAddress
            };

            var response = await handler.Handle(request, CancellationToken.None);

            response.Count.Should().Be(0);
            response.Assets.Should().HaveCount(0);
            dbFactory.Current.TakenTrack.ToArray().Should().HaveCount(1)
                .And.ContainSingle(x =>
                    x.IsRefunded == false &&
                    x.IsWithdrawn == true &&
                    x.DispenserId == dispenser.Id
                );
        }

        [Fact]
        internal async Task WhenAssetNotFound_ShouldReturnsEmptyArray()
        {
            var dbFactory = new MockDbContextFactory(seed: true);
            var dispenser = dbFactory.Current.Dispenser.First();
            var dispenserContract = MockDispenserProviderContract.Create(dispenser, isWithdrawn: false, isRefunded: false);
            var takenTrackManager = new TakenTrackManager(dbFactory, new AssetAvailabilityValidator(dispenserContract));
            var handler = new ListOfAssetsHandler(dbFactory, takenTrackManager);

            var request = new ListOfAssetsRequest
            {
                UserAddress = "0x0000000000000000000000000000000000000101"
            };

            var response = await handler.Handle(request, CancellationToken.None);

            response.Count.Should().Be(0);
            response.Assets.Should().BeEmpty();
            dbFactory.Current.TakenTrack.ToArray().Should().BeEmpty();
        }
    }
}