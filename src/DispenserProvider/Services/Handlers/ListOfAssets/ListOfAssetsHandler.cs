using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.Services.Database;
using DispenserProvider.Services.Handlers.ListOfAssets.Models;

namespace DispenserProvider.Services.Handlers.ListOfAssets;

public class ListOfAssetsHandler(IDbContextFactory<DispenserContext> dispenserContextFactory, ITakenTrackManager takenTrackManager) : IRequestHandler<ListOfAssetsRequest, ListOfAssetsResponse>
{
    public ListOfAssetsResponse Handle(ListOfAssetsRequest request)
    {
        var dispenserContext = dispenserContextFactory.CreateDbContext();
        var dispensers = dispenserContext.Dispenser
            .Where(x =>
                x.UserAddress == request.UserAddress.Address &&
                x.DeletionLogSignature == null &&
                x.TakenTrack == null
            )
            .Include(x => x.WithdrawalDetail)
                .ThenInclude(x => x.Builders)
            .Include(x => x.RefundDetail)
                .ThenInclude(x => x!.Builders)
            .ToArray();

        takenTrackManager.ProcessTakenTracks(dispensers);

        return new ListOfAssetsResponse(dispensers.Select(x => new Asset(x)));
    }
}