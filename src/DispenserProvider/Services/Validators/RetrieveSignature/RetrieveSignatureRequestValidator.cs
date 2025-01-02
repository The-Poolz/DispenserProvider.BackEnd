using FluentValidation;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Validators.GenerateSignature;

namespace DispenserProvider.Services.Validators.RetrieveSignature;

public class RetrieveSignatureRequestValidator : AbstractValidator<DispenserDTO>
{
    public RetrieveSignatureRequestValidator(
        AssetAvailabilityValidator assetValidator
    )
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.LastUserSignature)
            .NotNull()
            .WithMessage("Signature for user, not found.")
            .Must(x => DateTime.UtcNow >= x!.ValidFrom)
            .WithMessage(x => $"Cannot retrieve signature, because the valid time for retrieving has not yet arrived. Please try again after ({x.LastUserSignature!.ValidFrom}).")
            .Must(x => DateTime.UtcNow <= x!.ValidUntil)
            .WithMessage(x => $"Cannot retrieve signature, because the valid time ({x.LastUserSignature!.ValidUntil}) for using signature is expired.");

        RuleFor(x => x)
            .SetValidator(assetValidator);
    }
}