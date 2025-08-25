using FluentValidation;
using DispenserProvider.DataBase.Models;
using Net.Utils.ErrorHandler.Extensions;

namespace DispenserProvider.Services.Validators.Signature;

public class RefundSignatureValidator : AbstractValidator<DispenserDTO>
{
    public RefundSignatureValidator()
    {
        RuleFor(x => x)
            .Must(x => DateTime.UtcNow <= x.RefundFinishTime)
            .WithState(x => new
            {
                RefundFinishTime = x.RefundFinishTime!.Value.ToUnixTimeSeconds()
            })
            .WithError(ErrorCode.REFUND_TIME_IS_EXPIRED);
    }
}