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
            .WithMessage("Signature for user, not found.");

        RuleFor(x => x.Dispenser.LastUserSignature)
            .Must(x => DateTime.UtcNow >= x!.ValidFrom)
            .WithMessage(x =>
                $"Cannot retrieve signature, because the valid time for retrieving has not yet arrived. Please try again after ({x.Dispenser.LastUserSignature!.ValidFrom})."
            )
            .Must(x => DateTime.UtcNow <= x!.ValidUntil)
            .WithMessage(x =>
                $"Cannot retrieve signature, because the valid time ({x.Dispenser.LastUserSignature!.ValidUntil}) for using signature is expired."
            );

        RuleFor(x => x)
            .Must(x =>
                x.IsRefund == x.Dispenser.LastUserSignature!.IsRefund ||
                !x.IsRefund == !x.Dispenser.LastUserSignature!.IsRefund
            )
            .WithMessage(x =>
                $"Cannot retrieve signature, because it was generated for a {(x.IsRefund == x.Dispenser.LastUserSignature!.IsRefund ? "refund" : "withdraw")} operation."
            );

        RuleFor(x => x.Dispenser)
            .SetValidator(assetValidator);
    }
}