using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models.DatabaseWrappers;

public sealed class DispenserWrapper : DispenserDTO
{
    public DispenserWrapper(CreateAssetRequest request, TransactionDetailDTO withdrawalDetails, TransactionDetailDTO? refundDetails)
        : base(withdrawalDetails, refundDetails)
    {
        CreationLogSignature = request.Signature;
        RefundFinishTime = request.Message.Refund?.FinishTime;
        WithdrawalDetail = withdrawalDetails;
        RefundDetail = refundDetails;
    }
}