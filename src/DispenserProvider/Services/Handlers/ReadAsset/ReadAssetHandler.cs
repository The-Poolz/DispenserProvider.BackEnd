using MediatR;
using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.Services.Database;
using DispenserProvider.Services.Handlers.ReadAsset.Models;

namespace DispenserProvider.Services.Handlers.ReadAsset;

public class ReadAssetHandler(IDbContextFactory<DispenserContext> dispenserContextFactory, ITakenTrackManager takenTrackManager) : IRequestHandler<ReadAssetRequest, ReadAssetResponse>
{
    public Task<ReadAssetResponse> Handle(ReadAssetRequest request, CancellationToken cancellationToken)
    {
        var dispenserContext = dispenserContextFactory.CreateDbContext();
        var assets = request.AssetContext.Select(assetContext =>
            new Asset(assetContext, dispenserContext.Dispenser
                .Include(x => x.TakenTrack)
                .Include(x => x.WithdrawalDetail)
                .ThenInclude(x => x.Builders)
                .Include(x => x.RefundDetail)
                .ThenInclude(x => x!.Builders)
                .Where(x =>
                    x.DeletionLogSignature == null &&
                    ((x.WithdrawalDetail.ChainId == assetContext.ChainId && x.WithdrawalDetail.PoolId == assetContext.PoolId) ||
                     (x.RefundDetail != null && x.RefundDetail.ChainId == assetContext.ChainId && x.RefundDetail.PoolId == assetContext.PoolId))
                )
                .ToArray()
                .Select(x => new Dispenser(
                    x,
                    x.RefundDetail != null && x.RefundDetail.ChainId == assetContext.ChainId && x.RefundDetail.PoolId == assetContext.PoolId
                ))
            )
        ).ToList();

        takenTrackManager.ProcessTakenTracks(assets.SelectMany(a => a.Dispensers.Select(d => d.DTO)));

        return Task.FromResult(new ReadAssetResponse(assets));
    }
}
