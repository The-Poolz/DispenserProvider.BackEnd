using Nethereum.Util;
using FluentValidation;
using DispenserProvider.Services.Validators.Signature.Models;

namespace DispenserProvider.Services.Validators.Signature;

public class RetrieveSignatureRequestValidator : AbstractValidator<RetrieveSignatureValidatorRequest>
{
    public RetrieveSignatureRequestValidator(AssetAvailabilityValidator assetValidator)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Dispenser.LastUserSignature)
            .NotNull()
            .WithErrorCode(ErrorCode.SIGNATURE_NOT_FOUND.ToString())
            .WithMessage("Signature for user, not found.");

        RuleFor(x => x.Dispenser.LastUserSignature)
            .Must(x => DateTime.UtcNow >= x!.ValidFrom)
            .WithState(x => new
            {
                ValidFrom = x.Dispenser.LastUserSignature!.ValidFrom.ToUnixTimestamp()
            })
            .WithErrorCode(ErrorCode.SIGNATURE_VALID_TIME_NOT_ARRIVED.ToString())
            .WithMessage(x =>
                $"Cannot retrieve signature, because the valid time for retrieving has not yet arrived. Please try again after ({x.Dispenser.LastUserSignature!.ValidFrom.ToUnixTimestamp()})."
            )
            .Must(x => DateTime.UtcNow <= x!.ValidUntil)
            .WithState(x => new
            {
                ValidUntil = x.Dispenser.LastUserSignature!.ValidUntil.ToUnixTimestamp()
            })
            .WithErrorCode(ErrorCode.SIGNATURE_VALID_TIME_IS_EXPIRED.ToString())
            .WithMessage(x =>
                $"Cannot retrieve signature, because the valid time ({x.Dispenser.LastUserSignature!.ValidUntil.ToUnixTimestamp()}) for using signature is expired."
            );

        RuleFor(x => x)
            .Must(x =>
                x.IsRefund == x.Dispenser.LastUserSignature!.IsRefund ||
                !x.IsRefund == !x.Dispenser.LastUserSignature!.IsRefund
            )
            .WithState(x => new
            {
                IsRefund = x.IsRefund == x.Dispenser.LastUserSignature!.IsRefund
            })
            .WithErrorCode(ErrorCode.SIGNATURE_TYPE_IS_INVALID.ToString())
            .WithMessage(x =>
                $"Cannot retrieve signature, because it was generated for a {(x.IsRefund == x.Dispenser.LastUserSignature!.IsRefund ? "refund" : "withdraw")} operation."
            );

        RuleFor(x => x.Dispenser)
            .SetValidator(assetValidator);
    }
}