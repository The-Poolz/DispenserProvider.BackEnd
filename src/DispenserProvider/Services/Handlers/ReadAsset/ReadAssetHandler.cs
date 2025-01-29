using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.Services.Validators.Signature;
using DispenserProvider.Services.Handlers.ReadAsset.Models;
using DispenserProvider.Services.Handlers.ListOfAssets.Models.DatabaseWrappers;

namespace DispenserProvider.Services.Handlers.ReadAsset;

public class ReadAssetHandler(IDbContextFactory<DispenserContext> dispenserContextFactory, AssetAvailabilityValidator assetValidator) : IRequestHandler<ReadAssetRequest, ReadAssetResponse>
{
    public ReadAssetResponse Handle(ReadAssetRequest request)
    {
        var dispenserContext = dispenserContextFactory.CreateDbContext();
        var assets = request.AssetContext.Select(assetContext =>
            new Asset(assetContext, dispenserContext.TransactionDetails
                .Where(x =>
                    x.PoolId == assetContext.PoolId &&
                    x.ChainId == assetContext.ChainId
                )
                .Include(x => x.WithdrawalDispenser)
                    .ThenInclude(x => x!.TakenTrack)
                .Include(x => x.RefundDispenser)
                    .ThenInclude(x => x!.TakenTrack)
                .Include(x => x.Builders)
                .ToArray()
                .Where(x =>
                    x.WithdrawalDispenser?.DeletionLogSignature == null &&
                    x.RefundDispenser?.DeletionLogSignature == null
                )
                .Select(x => new Dispenser(
                    x.RefundDispenser != null ? x.RefundDispenser! : x.WithdrawalDispenser!,
                    x.Builders
                ))
                .ToArray()
            )
        ).ToList();

        var isTracksAdded = false;
        assets.ForEach(asset =>
        {
            asset.Dispensers.ToList().ForEach(dispenser =>
            {
                if (dispenser.IsTaken) return;

                var validation = assetValidator.Validate(dispenser.DTO);
                if (validation.IsValid) return;

                dispenserContext.TakenTrack.Add(new TakenTrack(validation.Errors[0].ErrorCode, dispenser.DTO));
                isTracksAdded = true;
            });
        });
        if (isTracksAdded) dispenserContext.SaveChanges();

        return new ReadAssetResponse(assets);
    }
}
