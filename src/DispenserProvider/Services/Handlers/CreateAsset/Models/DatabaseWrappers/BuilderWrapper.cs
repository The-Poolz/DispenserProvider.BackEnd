using DispenserProvider.Extensions;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models.DatabaseWrappers;

public sealed class BuilderWrapper : BuilderDTO
{
    public BuilderWrapper(User user, TransactionDetailWrapper transactionDetail, Schedule schedule)
    {
        ProviderAddress = schedule.ProviderAddress;
        StartTime = schedule.StartDate;
        FinishTime = schedule.FinishDate;
        WeiAmount = user.WeiAmount.MultiplyWeiByRatio(schedule.Ratio.StringRatioToDecimal());
        TransactionDetail = transactionDetail;
    }

    public BuilderWrapper(User user, TransactionDetailWrapper transactionDetail, Refund refund)
    {
        ProviderAddress = refund.DealProvider;
        WeiAmount = user.WeiAmount.MultiplyWeiByRatio(refund.Ratio.StringRatioToDecimal());
        TransactionDetail = transactionDetail;
    }
}