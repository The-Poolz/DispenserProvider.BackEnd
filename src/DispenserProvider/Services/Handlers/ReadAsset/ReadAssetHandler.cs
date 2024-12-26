using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.Services.Handlers.ReadAsset.Models;

namespace DispenserProvider.Services.Handlers.ReadAsset;

public class ReadAssetHandler(DispenserContext dispenserContext) : IRequestHandler<ReadAssetRequest, ReadAssetResponse>
{
    public ReadAssetResponse Handle(ReadAssetRequest request)
    {
        var assets = request.AssetContext.Select(assetContext =>
            new Asset(assetContext, dispenserContext.TransactionDetails
                .Where(x =>
                    x.PoolId == assetContext.PoolId &&
                    x.ChainId == assetContext.ChainId
                )
                .Include(x => x.WithdrawalDispenser)
                .Include(x => x.RefundDispenser)
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
