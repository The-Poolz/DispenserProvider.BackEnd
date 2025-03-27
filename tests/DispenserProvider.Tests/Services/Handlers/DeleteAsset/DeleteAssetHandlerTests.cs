using Xunit;
using System.Net;
using FluentAssertions;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Services.Handlers.DeleteAsset;
using DispenserProvider.Tests.Mocks.Services.Handlers.DeleteAsset.Models;

namespace DispenserProvider.Tests.Services.Handlers.DeleteAsset;

public class DeleteAssetHandlerTests
{
    public class Handle
    {
        [Fact]
        internal async Task WhenMarkedAsDeletedSuccessfully_ShouldContextUpdatedSuccessfully()
        {
            var dbFactory = new MockDbContextFactory(seed: true);
            var handler = new DeleteAssetHandler(dbFactory);

            var response = await handler.Handle(MockDeleteAssetRequest.Request, CancellationToken.None);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            dbFactory.Current.Logs.ToArray().Should().ContainSingle(x =>
                x.Signature == MockDeleteAssetRequest.Request.Signature &&
                x.IsCreation == false
            );
            dbFactory.Current.Dispenser.ToArray().Should().ContainSingle(x =>
                x.Id == MockDispenserContext.Dispenser.Id &&
                x.RefundFinishTime == MockDispenserContext.Dispenser.RefundFinishTime &&
                x.DeletionLogSignature == MockDeleteAssetRequest.Request.Signature
            );
        }
    }
}