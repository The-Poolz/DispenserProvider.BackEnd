using Nethereum.Util;
using FluentValidation;
using DispenserProvider.Extensions;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Validators.Signature;

public class RefundSignatureValidator : AbstractValidator<DispenserDTO>
{
    public RefundSignatureValidator()
    {
        RuleFor(x => x)
            .Must(x => DateTime.UtcNow <= x.RefundFinishTime)
            .WithState(x => new
            {
                RefundFinishTime = x.RefundFinishTime!.Value.ToUnixTimestamp()
            })
            .WithError(ErrorCode.REFUND_TIME_IS_EXPIRED);
    }
}