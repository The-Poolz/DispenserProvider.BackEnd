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

        // Use a single timestamp to ensure consistency across all calculations
        var baseTimestamp = DateTime.UtcNow;
        var validUntil = CalculateValidUntil(dispenser.RefundFinishTime, isRefund, baseTimestamp);
        var validFrom = CalculateValidFrom(dispenser, baseTimestamp);
        
        var signature = new SignatureDTO
        {
            Signature = signatureGenerator.GenerateSignature(transactionDetail, validUntil),
            ValidUntil = validUntil,
            ValidFrom = validFrom,
            IsRefund = isRefund,
            DispenserId = dispenser.Id
        };

        var dispenserContext = dispenserContextFactory.CreateDbContext();
        dispenserContext.Signatures.Add(signature);
        dispenserContext.SaveChanges();

        return signature.ValidFrom;
    }

    private static DateTime CalculateValidFrom(DispenserDTO dispenser, DateTime baseTimestamp)
    {
        return dispenser.LastUserSignature != null
            ? baseTimestamp + TimeSpan.FromSeconds(Env.VALID_FROM_OFFSET_IN_SECONDS.GetRequired<int>())
            : baseTimestamp;
    }

    private static DateTime CalculateValidUntil(DateTime? refundFinishTime, bool isRefund, DateTime baseTimestamp)
    {
        var calculated = baseTimestamp + TimeSpan.FromSeconds(Env.VALID_UNTIL_MAX_OFFSET_IN_SECONDS.GetRequired<int>());
        return isRefund && calculated > refundFinishTime!.Value ? refundFinishTime.Value : calculated;
    }
}