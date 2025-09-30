using FluentValidation;
using DispenserProvider.Extensions;
using Net.Utils.ErrorHandler.Extensions;
using DispenserProvider.Services.Validators.Signature.Models;

namespace DispenserProvider.Services.Validators.Signature;

public class UpdatingSignatureValidator : AbstractValidator<GenerateSignatureValidatorRequest>
{
    public UpdatingSignatureValidator()
    {
        RuleFor(x => x.Dispenser.LastUserSignature!)
            .Cascade(CascadeMode.Stop)
            .Must(x => DateTime.UtcNow >= x.ValidUntil)
            .WithError(ErrorCode.SIGNATURE_IS_STILL_VALID, x => new
            {
                ValidFrom = x.Dispenser.LastUserSignature!.ValidFrom.ToUnixTimeSeconds(),
                NextTry = x.Dispenser.NextTry().ToUnixTimeSeconds()
            })
            .Must(x => DateTime.UtcNow >= x.NextTry())
            .When(x => x.IsRefund != x.Dispenser.LastUserSignature!.IsRefund)
            .WithError(ErrorCode.SIGNATURE_GENERATION_VALID_TIME_NOT_ARRIVED, x => new
            {
                NextTry = x.Dispenser.NextTry().ToUnixTimeSeconds()
            });
    }
}