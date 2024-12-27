using DispenserProvider.DataBase.Models;
using DispenserProvider.Extensions;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models.DatabaseWrappers;

public sealed class BuilderWrapper : BuilderDTO
{
    public BuilderWrapper(User user, TransactionDetailWrapper transactionDetail, Schedule schedule)
    {
        ProviderAddress = schedule.ProviderAddress;
        StartTime = schedule.StartDate;
        FinishTime = schedule.FinishDate;
        WeiAmount = user.CalculateAmount(schedule);
        TransactionDetail = transactionDetail;
    }

    public BuilderWrapper(User user, TransactionDetailWrapper transactionDetail, Refund refund)
    {
        ProviderAddress = refund.DealProvider;
        WeiAmount = user.CalculateAmount(refund);
        TransactionDetail = transactionDetail;
    }
}