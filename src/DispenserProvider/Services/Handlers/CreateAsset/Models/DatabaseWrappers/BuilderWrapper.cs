using System.Numerics;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models.DatabaseWrappers;

public sealed class BuilderWrapper : BuilderDTO
{
    public BuilderWrapper(User user, TransactionDetailWrapper transactionDetail, Schedule schedule)
    {
        ProviderAddress = schedule.ProviderAddress;
        StartTime = schedule.StartDate;
        FinishTime = schedule.FinishDate;
        WeiAmount = new BigInteger(schedule.Ratio * (decimal)BigInteger.Parse(user.WeiAmount)).ToString();
        TransactionDetail = transactionDetail;
    }

    public BuilderWrapper(User user, TransactionDetailWrapper transactionDetail, Refund refund)
    {
        ProviderAddress = refund.DealProvider;
        WeiAmount = new BigInteger(refund.Ratio * (decimal)BigInteger.Parse(user.WeiAmount)).ToString();
        TransactionDetail = transactionDetail;
    }
}