using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models.DatabaseWrappers;

public sealed class DispenserWrapper : DispenserDTO
{
    public DispenserWrapper(CreateAssetRequest request, User user, TransactionDetailDTO withdrawalDetails, TransactionDetailDTO? refundDetails)
        : base(user.UserAddress, withdrawalDetails, refundDetails)
    {
        CreationLogSignature = request.Signature;
        RefundFinishTime = request.Message.Refund?.FinishTime;
        UserAddress = user.UserAddress;
        WithdrawalDetail = withdrawalDetails;
        RefundDetail = refundDetails;
    }
}