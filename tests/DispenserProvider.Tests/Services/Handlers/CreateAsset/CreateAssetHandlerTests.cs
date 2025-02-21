using Xunit;
using System.Net;
using FluentAssertions;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Services.Handlers.CreateAsset;
using DispenserProvider.Tests.Mocks.Services.Handlers.CreateAsset.Models;

namespace DispenserProvider.Tests.Services.Handlers.CreateAsset;

public class CreateAssetHandlerTests
{
    public class Handle
    {
        [Fact]
        internal async Task WhenSavingSuccessfully_ShouldContextContainsExpectedEntities()
        {
            var dbFactory = new MockDbContextFactory();
            var handler = new CreateAssetHandler(dbFactory);

            var response = await handler.Handle(MockCreateAssetRequest.Request, CancellationToken.None);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            dbFactory.Current.Logs.ToArray().Should().ContainSingle(x =>
                x.Signature == MockDispenserContext.Log.Signature &&
                x.IsCreation == true
            );
            dbFactory.Current.Dispenser.ToArray().Should().ContainSingle(x =>
                x.Id == MockDispenserContext.Dispenser.Id &&
                x.UserAddress == MockDispenserContext.Dispenser.UserAddress &&
                x.RefundFinishTime == MockDispenserContext.Dispenser.RefundFinishTime
            );
            dbFactory.Current.TransactionDetails.ToArray().Should().ContainSingle(x =>
                x.Id == MockDispenserContext.TransactionDetail.Id &&
                x.PoolId == MockDispenserContext.TransactionDetail.PoolId &&
                x.ChainId == MockDispenserContext.TransactionDetail.ChainId
            );
            dbFactory.Current.Builders.ToArray().Should().HaveCount(2);
        }
    }
}