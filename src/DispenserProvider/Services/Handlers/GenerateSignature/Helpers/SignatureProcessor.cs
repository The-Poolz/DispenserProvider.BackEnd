using DispenserProvider.DataBase;
using EnvironmentManager.Extensions;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Helpers;

public class SignatureProcessor(DispenserContext dispenserContext, SignatureGenerator signatureGenerator)
{
    public DateTime SaveSignature(DispenserDTO dispenser, bool isRefund)
    {
        var transactionDetail = isRefund ? dispenser.RefundDetail! : dispenser.WithdrawalDetail = dispenser.WithdrawalDetail;

        var validUntil = CalculateValidUntil(dispenser.RefundFinishTime, isRefund);
        var signature = new SignatureDTO
        {
            Signature = signatureGenerator.GenerateSignature(transactionDetail, validUntil),
            ValidUntil = validUntil,
            ValidFrom = DateTime.UtcNow + TimeSpan.FromSeconds(Env.VALID_FROM_OFFSET_IN_SECONDS.GetRequired<int>()),
            IsRefund = isRefund,
            Dispenser = dispenser
        };

        dispenserContext.Signatures.Add(signature);
        dispenserContext.SaveChanges();

        return signature.ValidFrom;
    }

    private static DateTime CalculateValidUntil(DateTime? refundFinishTime, bool isRefund)
    {
        var calculated = DateTime.UtcNow + TimeSpan.FromSeconds(Env.VALID_UNTIL_MAX_OFFSET_IN_SECONDS.GetRequired<int>());
        return isRefund && calculated > refundFinishTime!.Value ? refundFinishTime.Value : calculated;
    }
}