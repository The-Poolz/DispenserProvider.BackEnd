using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.Services.Handlers.ReadAsset.Models;

namespace DispenserProvider.Services.Handlers.ReadAsset;

public class ReadAssetHandler(IDbContextFactory<DispenserContext> dispenserContextFactory) : IRequestHandler<ReadAssetRequest, ReadAssetResponse>
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
                    )
                )
                .ToArray()
            )
        ).ToArray();

        return new ReadAssetResponse(assets);
    }
}
