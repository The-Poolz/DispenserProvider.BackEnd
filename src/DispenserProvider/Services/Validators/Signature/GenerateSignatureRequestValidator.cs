using FluentValidation;
using DispenserProvider.Services.Validators.Signature.Models;

namespace DispenserProvider.Services.Validators.Signature;

public class GenerateSignatureValidator : AbstractValidator<GenerateSignatureValidatorRequest>
{
    public GenerateSignatureValidator(
        UpdatingSignatureValidator updatingValidator,
        RefundSignatureValidator refundValidator,
        AssetAvailabilityValidator assetValidator
    )
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Dispenser)
            .SetValidator(updatingValidator)
            .When(x => x.Dispenser.LastUserSignature != null);

        RuleFor(x => x.Dispenser)
            .SetValidator(refundValidator)
            .When(x => x.IsRefund);

        RuleFor(x => x.Dispenser)
            .SetValidator(assetValidator);
    }
}