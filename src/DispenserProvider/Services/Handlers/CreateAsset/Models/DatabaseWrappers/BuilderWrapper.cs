using DispenserProvider.Extensions;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models.DatabaseWrappers;

public sealed class BuilderWrapper : BuilderDTO
{
    public BuilderWrapper(User user, TransactionDetailWrapper transactionDetail, Schedule schedule)
    {
        ProviderAddress = schedule.ProviderAddress;
        StartTime = schedule.StartDate.UtcDateTime;
        FinishTime = schedule.FinishDate.ToUnixTimeSeconds() == 0 ? null : schedule.FinishDate.UtcDateTime;
        WeiAmount = user.WeiAmount.MultiplyWeiByRatio(schedule.WeiRatio);
        TransactionDetail = transactionDetail;
    }

    public BuilderWrapper(User user, TransactionDetailWrapper transactionDetail, Refund refund)
    {
        ProviderAddress = refund.DealProvider;
        WeiAmount = user.WeiAmount.MultiplyWeiByRatio(refund.WeiRatio);
        TransactionDetail = transactionDetail;
    }
}