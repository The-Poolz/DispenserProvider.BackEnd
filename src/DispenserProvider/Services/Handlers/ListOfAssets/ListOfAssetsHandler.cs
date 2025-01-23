using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Validators.Signature;
using DispenserProvider.Services.Handlers.ListOfAssets.Models;
using DispenserProvider.Services.Handlers.ListOfAssets.Models.DatabaseWrappers;

namespace DispenserProvider.Services.Handlers.ListOfAssets;

public class ListOfAssetsHandler(IDbContextFactory<DispenserContext> dispenserContextFactory, AssetAvailabilityValidator assetValidator) : IRequestHandler<ListOfAssetsRequest, ListOfAssetsResponse>
{
    public ListOfAssetsResponse Handle(ListOfAssetsRequest request)
    {
        using var dispenserContext = dispenserContextFactory.CreateDbContext();
        var dispensers = dispenserContext.Dispenser
            .Where(x =>
                x.UserAddress == request.UserAddress.Address &&
                x.DeletionLogSignature == null
            )
            .Include(x => x.WithdrawalDetail)
                .ThenInclude(x => x.Builders)
            .Include(x => x.RefundDetail)
                .ThenInclude(x => x!.Builders)
            .ToList();

        var takenDispensers = new List<DispenserDTO>();
        foreach (var dispenser in dispensers)
        {
            var isTaken = assetValidator.Validate(dispenser);
            if (isTaken.Errors.Count > 0)
            {
                takenDispensers.Add(dispenser);
                dispenserContext.TakenTrack.Add(new TakenTrack(isTaken.Errors[0].ErrorCode, dispenser));
            }
        }
        if (takenDispensers.Count > 0) dispenserContext.SaveChanges();

        var assets = dispensers.Except(takenDispensers)
            .Select(dispenser => new Asset(dispenser))
            .ToArray();

        return new ListOfAssetsResponse(assets);
    }
}