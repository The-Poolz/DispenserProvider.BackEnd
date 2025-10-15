using MediatR;
using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.Services.Database;
using DispenserProvider.Services.Handlers.ReadAsset.Models;
using DispenserProvider.Extensions;

namespace DispenserProvider.Services.Handlers.ReadAsset;

public class ReadAssetHandler(IDbContextFactory<DispenserContext> dispenserContextFactory, ITakenTrackManager takenTrackManager) : IRequestHandler<ReadAssetRequest, ReadAssetResponse>
{
    public Task<ReadAssetResponse> Handle(ReadAssetRequest request, CancellationToken cancellationToken)
    {
        var dispenserContext = dispenserContextFactory.CreateDbContext();
        var assets = request.AssetContext.Select(assetContext =>
            new Asset(assetContext, dispenserContext.Dispenser
                .Include(dispenser => dispenser.TakenTrack)
                .Include(dispenser => dispenser.WithdrawalDetail)
                .ThenInclude(transactionDetail => transactionDetail.Builders)
                .Include(dispenser => dispenser.RefundDetail)
                .ThenInclude(transactionDetail => transactionDetail!.Builders)
                .Where(dispenser =>
                    dispenser.DeletionLogSignature == null &&
                    ((dispenser.WithdrawalDetail.ChainId == assetContext.ChainId && dispenser.WithdrawalDetail.PoolId == assetContext.PoolId) ||
                     (dispenser.RefundDetail != null && dispenser.RefundDetail.ChainId == assetContext.ChainId && dispenser.RefundDetail.PoolId == assetContext.PoolId))
                )
                .ToArray()
                .Select(dispenser => new Dispenser(
                    dispenser,
                    dispenser.RefundDetail != null && dispenser.RefundDetail.ChainId == assetContext.ChainId && dispenser.RefundDetail.PoolId == assetContext.PoolId
                ))
            )
        )
        .AsEnumerable()
        .Where(x => !TestnetChainsManager.TestnetChains.Contains(x.ChainId))
        .ToList();

        takenTrackManager.ProcessTakenTracks(assets.SelectMany(a => a.Dispensers.Select(d => d.DTO)).ToArray());

        return Task.FromResult(new ReadAssetResponse(assets));
    }
}
