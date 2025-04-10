﻿using Xunit;
using FluentAssertions;
using DispenserProvider.Services.Database;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Tests.Mocks.Services.Web3;
using DispenserProvider.Services.Handlers.ReadAsset;
using DispenserProvider.Services.Validators.Signature;
using DispenserProvider.Services.Handlers.ReadAsset.Models;

namespace DispenserProvider.Tests.Services.Handlers.ReadAsset;

public class ReadAssetHandlerTests
{
    public class Handle
    {
        [Fact]
        internal async Task WhenAssetFound_ShouldReturnsExpectedAsset()
        {
            var dbFactory = new MockDbContextFactory(seed: true);
            var dispenser = dbFactory.Current.Dispenser.First();
            var dispenserContract = MockDispenserProviderContract.Create(dispenser, isWithdrawn: false, isRefunded: false);
            var takenTrackManager = new TakenTrackManager(dbFactory, new AssetAvailabilityValidator(dispenserContract));
            var handler = new ReadAssetHandler(dbFactory, takenTrackManager);

            var request = new ReadAssetRequest
            {
                AssetContext = [
                    new AssetContext {
                        PoolId = 1,
                        ChainId = 97
                    }
                ]
            };

            var response = await handler.Handle(request, CancellationToken.None);

            response.Assets.Should().HaveCount(1)
                .And.ContainSingle(asset =>
                    asset.ChainId == request.AssetContext.First().ChainId &&
                    asset.PoolId == request.AssetContext.First().PoolId
                );

            response.Assets.First().Dispensers.Should().HaveCount(1)
                .And.ContainSingle(d =>
                    d.UserAddress == MockDispenserContext.TransactionDetail.UserAddress &&
                    d.RefundFinishTime == MockDispenserContext.Dispenser.RefundFinishTime
                );

            response.Assets.First().Dispensers.First().Builders.Should().HaveCount(1)
                .And.ContainSingle(builder =>
                    builder.ProviderAddress == MockDispenserContext.Builder.ProviderAddress &&
                    builder.WeiAmount == MockDispenserContext.Builder.WeiAmount &&
                    builder.StartTime == MockDispenserContext.Builder.StartTime &&
                    builder.FinishTime == MockDispenserContext.Builder.FinishTime
                );
        }

        [Fact]
        internal async Task WhenAssetIsTracked_ShouldReturnsEmptyCollection()
        {
            var dbFactory = new MockDbContextFactory(seed: true);
            var dispenser = dbFactory.Current.Dispenser.First();
            var dispenserContract = MockDispenserProviderContract.Create(dispenser, isWithdrawn: true, isRefunded: false);
            var takenTrackManager = new TakenTrackManager(dbFactory, new AssetAvailabilityValidator(dispenserContract));
            var handler = new ReadAssetHandler(dbFactory, takenTrackManager);

            var request = new ReadAssetRequest
            {
                AssetContext = [
                    new AssetContext {
                        PoolId = 1,
                        ChainId = 97
                    }
                ]
            };

            var response = await handler.Handle(request, CancellationToken.None);

            response.Assets.Should().HaveCount(1)
                .And.ContainSingle(asset =>
                    asset.ChainId == request.AssetContext.First().ChainId &&
                    asset.PoolId == request.AssetContext.First().PoolId
                );

            response.Assets.First().Dispensers.Should().HaveCount(1)
                .And.ContainSingle(d =>
                    d.UserAddress == MockDispenserContext.TransactionDetail.UserAddress &&
                    d.RefundFinishTime == MockDispenserContext.Dispenser.RefundFinishTime
                );

            response.Assets.First().Dispensers.First().Builders.Should().HaveCount(1)
                .And.ContainSingle(builder =>
                    builder.ProviderAddress == MockDispenserContext.Builder.ProviderAddress &&
                    builder.WeiAmount == MockDispenserContext.Builder.WeiAmount &&
                    builder.StartTime == MockDispenserContext.Builder.StartTime &&
                    builder.FinishTime == MockDispenserContext.Builder.FinishTime
                );

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
            var handler = new ReadAssetHandler(dbFactory, takenTrackManager);

            var request = new ReadAssetRequest
            {
                AssetContext = [
                    new AssetContext {
                        PoolId = 123,
                        ChainId = 123
                    }
                ]
            };

            var response = await handler.Handle(request, CancellationToken.None);

            response.Assets.Should().HaveCount(1)
                .And.ContainSingle(asset =>
                    asset.ChainId == request.AssetContext.First().ChainId &&
                    asset.PoolId == request.AssetContext.First().PoolId
                );

            response.Assets.First().Dispensers.Should().HaveCount(0);
        }
    }
}