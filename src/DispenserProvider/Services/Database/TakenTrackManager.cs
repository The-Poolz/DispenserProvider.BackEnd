using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Web3.MultiCall;
using DispenserProvider.Services.Database.Models;
using DispenserProvider.Services.Web3.MultiCall.Models;

namespace DispenserProvider.Services.Database;

public class TakenTrackManager(IDbContextFactory<DispenserContext> dispenserContextFactory, IMultiCallContract multiCall) : ITakenTrackManager
{
    public IEnumerable<DispenserDTO> ProcessTakenTracks(ICollection<DispenserDTO> dispensers)
    {
        var dispenserContext = dispenserContextFactory.CreateDbContext();

        var notTakenDispensers = dispensers
            .Where(dispenser => dispenser.TakenTrack == null)
            .ToList();

        var isTakenBatchRequests = UniqueChainIDs(notTakenDispensers).Select(chainId =>
        {
            var withdrawsIsTakenRequests = dispensers
                .Where(x => x.WithdrawalDetail.ChainId == chainId)
                .Select(x => new IsTakenRequest(x.Id, x.WithdrawalDetail.PoolId, x.WithdrawalDetail.UserAddress, false));

            var refundsIsTakenRequests = dispensers
                .Where(x => x.RefundDetail?.ChainId == chainId)
                .Select(x => new IsTakenRequest(x.Id, x.RefundDetail!.PoolId, x.RefundDetail.UserAddress, true));

            var isTakenRequests = withdrawsIsTakenRequests
                .Concat(refundsIsTakenRequests)
                .ToArray();

            return new MultiCallRequest(chainId, isTakenRequests);
        }).ToArray();

        var onlyTaken = multiCall
            .IsTakenBatchAsync(isTakenBatchRequests)
            .GetAwaiter()
            .GetResult()
            .SelectMany(r => r.IsTakenResponses)
            .Where(x => x.IsTaken);

        var processed = new List<DispenserDTO>();
        foreach (var isTakenResponse in onlyTaken)
        {
            var dispenser = notTakenDispensers.FirstOrDefault(x => x.Id == isTakenResponse.DispenserId);
            if (dispenser == null) continue;

            processed.Add(dispenser);
            dispenserContext.TakenTrack.Add(new TakenTrack(isTakenResponse.IsRefund, dispenser));
        }

        if (processed.Count > 0) dispenserContext.SaveChanges();

        return processed;
    }

    private static HashSet<long> UniqueChainIDs(ICollection<DispenserDTO> dispensers)
    {
        return dispensers
            .Select(x => x.WithdrawalDetail.ChainId)
            .Concat(dispensers.Where(x => x.RefundDetail != null).Select(x => x.RefundDetail!.ChainId))
            .ToHashSet();
    }
}