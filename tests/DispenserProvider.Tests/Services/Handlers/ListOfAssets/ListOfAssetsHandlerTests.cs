using Xunit;
using FluentAssertions;
using DispenserProvider.DataBase.Models;
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
            var secondDispenser = new DispenserDTO(MockDispenserContext.TransactionDetail.UserAddress, withdrawChainId: 1, withdrawPoolId: 1)
            {
                CreationLog = new LogDTO {
                    Signature = "0x"
                },
                WithdrawalDetail = new TransactionDetailDTO {
                    Id = 2,
                    UserAddress = MockDispenserContext.TransactionDetail.UserAddress,
                    ChainId = 1,
                    PoolId = 1
                }
            };
            dbFactory.Current.Dispenser.Add(secondDispenser);
            await dbFactory.Current.SaveChangesAsync();

            var dispenser = dbFactory.Current.Dispenser.First();
            var dispenserContract = MockDispenserProviderContract.Create(dispenser, isWithdrawn: false, isRefunded: false);
            var takenTrackManager = new TakenTrackManager(dbFactory, new AssetAvailabilityValidator(dispenserContract));
            var handler = new ListOfAssetsHandler(dbFactory, takenTrackManager);

            var request = new ListOfAssetsRequest
            {
                UserAddress = MockDispenserContext.TransactionDetail.UserAddress,
                Limit = 1,
                Page = 1
            };

            var response = await handler.Handle(request, CancellationToken.None);

            response.TotalAssets.Should().Be(2);
            response.Assets.Should().HaveCount(1)
                .And.ContainSingle(x =>
                    x.UserAddress == MockDispenserContext.TransactionDetail.UserAddress &&
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
                UserAddress = MockDispenserContext.TransactionDetail.UserAddress
            };

            var response = await handler.Handle(request, CancellationToken.None);

            response.TotalAssets.Should().Be(0);
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

            response.TotalAssets.Should().Be(0);
            response.Assets.Should().BeEmpty();
            dbFactory.Current.TakenTrack.ToArray().Should().BeEmpty();
        }
    }
}