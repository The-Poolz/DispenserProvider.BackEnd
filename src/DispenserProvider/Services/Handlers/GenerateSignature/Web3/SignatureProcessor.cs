using DispenserProvider.DataBase;
using EnvironmentManager.Extensions;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Web3;

public class SignatureProcessor(IDbContextFactory<DispenserContext> dispenserContextFactory, ISignatureGenerator signatureGenerator) : ISignatureProcessor
{
    public DateTime SaveSignature(DispenserDTO dispenser, bool isRefund)
    {
        var transactionDetail = isRefund ? dispenser.RefundDetail! : dispenser.WithdrawalDetail;

        var validUntil = CalculateValidUntil(dispenser.RefundFinishTime, isRefund);
        var signature = new SignatureDTO
        {
            Signature = signatureGenerator.GenerateSignature(transactionDetail, validUntil),
            ValidUntil = validUntil,
            ValidFrom = DateTime.UtcNow + TimeSpan.FromSeconds(Env.VALID_FROM_OFFSET_IN_SECONDS.GetRequired<int>()),
            IsRefund = isRefund,
            DispenserId = dispenser.Id
        };

        var dispenserContext = dispenserContextFactory.CreateDbContext();
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