using DispenserProvider.Extensions;
using FluentValidation;
using Net.Utils.ErrorHandler.Extensions;
using DispenserProvider.Services.Validators.Signature.Models;

namespace DispenserProvider.Services.Validators.Signature;

public class RetrieveSignatureRequestValidator : AbstractValidator<RetrieveSignatureValidatorRequest>
{
    public RetrieveSignatureRequestValidator(AssetAvailabilityValidator assetValidator)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Dispenser.LastUserSignature)
            .NotNull()
            .WithError(ErrorCode.SIGNATURE_NOT_FOUND);

        RuleFor(x => x.Dispenser.LastUserSignature)
            .Must(x => DateTime.UtcNow >= x!.ValidFrom)
            .WithError(ErrorCode.SIGNATURE_VALID_TIME_NOT_ARRIVED, x => new
            {
                ValidFrom = x.Dispenser.LastUserSignature!.ValidFrom.ToUnixTimeSeconds()
            })
            .Must(x => DateTime.UtcNow <= x!.ValidUntil)
            .WithError(ErrorCode.SIGNATURE_VALID_TIME_IS_EXPIRED, x => new
            {
                ValidUntil = x.Dispenser.LastUserSignature!.ValidUntil.ToUnixTimeSeconds()
            });

        RuleFor(x => x)
            .Must(x =>
                x.IsRefund == x.Dispenser.LastUserSignature!.IsRefund ||
                !x.IsRefund == !x.Dispenser.LastUserSignature!.IsRefund
            )
            .WithError(ErrorCode.SIGNATURE_TYPE_IS_INVALID, x => new
            {
                IsRefund = x.IsRefund == x.Dispenser.LastUserSignature!.IsRefund,
                NextTry = x.Dispenser.NextTry()
            });

        RuleFor(x => x.Dispenser)
            .SetValidator(assetValidator);
    }
}