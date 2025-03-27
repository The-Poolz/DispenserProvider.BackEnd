using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models.DatabaseWrappers;

public class TransactionDetailWrapper : TransactionDetailDTO
{
    public TransactionDetailWrapper(CreateAssetMessage message, User user)
    {
        UserAddress = user.UserAddress;
        ChainId = message.ChainId;
        PoolId = message.PoolId;
    }

    public TransactionDetailWrapper(Refund refund, User user)
    {
        UserAddress = user.UserAddress;
        ChainId = refund.ChainId;
        PoolId = refund.PoolId;
    }
}