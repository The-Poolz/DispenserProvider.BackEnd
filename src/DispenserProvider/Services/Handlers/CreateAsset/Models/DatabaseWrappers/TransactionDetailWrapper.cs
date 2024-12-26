using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models.DatabaseWrappers;

public class TransactionDetailWrapper : TransactionDetailDTO
{
    public TransactionDetailWrapper(CreateAssetMessage message)
    {
        ChainId = message.ChainId;
        PoolId = message.PoolId;
    }

    public TransactionDetailWrapper(Refund refund)
    {
        ChainId = refund.ChainId;
        PoolId = refund.PoolId;
    }
}