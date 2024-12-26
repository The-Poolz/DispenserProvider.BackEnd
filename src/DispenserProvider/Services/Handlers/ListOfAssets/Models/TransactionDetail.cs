using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.ListOfAssets.Models;

public class TransactionDetail(TransactionDetailDTO transactionDetail)
{
    public long ChainId { get; } = transactionDetail.ChainId;
    public long PoolId { get; } = transactionDetail.PoolId;
    public Builder[] Builders { get; } = transactionDetail.Builders.Select(x => new Builder(x)).ToArray();
}