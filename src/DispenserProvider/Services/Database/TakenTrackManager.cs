using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Web3.MultiCall;
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

        var uniqueChainIDs = UniqueChainIDs(notTakenDispensers);
        var isTakenBatchRequests = uniqueChainIDs.Select(chainId =>
        {
            var withdrawsIsTakenRequests = dispensers
                .Where(x => x.WithdrawalDetail.ChainId == chainId)
                .Select(x => new IsTakenRequest(x.Id, x.WithdrawalDetail.PoolId, x.WithdrawalDetail.UserAddress, false));

            var refundsIsTakenRequests = dispensers
                .Where(x => x.RefundDetail != null)
                .Where(x => x.RefundDetail!.ChainId == chainId)
                .Select(x => new IsTakenRequest(x.Id ,x.RefundDetail!.PoolId, x.RefundDetail.UserAddress, false));

            var isTakenRequests = withdrawsIsTakenRequests
                .Concat(refundsIsTakenRequests)
                .ToArray();

            return new MultiCallRequest(chainId, isTakenRequests);
        }).ToArray();

        var isTakenBatchResponses = multiCall
            .IsTakenBatchAsync(isTakenBatchRequests)
            .GetAwaiter()
            .GetResult();

        var processed = new List<DispenserDTO>();
        // TODO: Process response of `isTakenBatchResponses` if marked as IsTaken add into `precessed`, also add new entry `dispenserContext.TakenTrack.Add(new TakenTrack(..., ...))`

        if (processed.Count > 0) dispenserContext.SaveChanges();

        return processed;
    }

    private static HashSet<long> UniqueChainIDs(ICollection<DispenserDTO> dispensers)
    {
        var uniqueWithdrawChainIDs = dispensers
            .Select(x => x.WithdrawalDetail.ChainId)
            .Distinct();

        var uniqueRefundChainIDs = dispensers
            .Where(x => x.RefundDetail != null)
            .Select(x => x.RefundDetail!.ChainId)
            .Distinct();
        
        var uniqueChainIDs = uniqueWithdrawChainIDs
            .Concat(uniqueRefundChainIDs)
            .Distinct();

        return uniqueChainIDs.ToHashSet();
    }
}