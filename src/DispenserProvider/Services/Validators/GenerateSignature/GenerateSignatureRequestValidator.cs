using FluentValidation;
using DispenserProvider.Services.Validators.GenerateSignature.Models;

namespace DispenserProvider.Services.Validators.GenerateSignature;

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
            .When(x => x.Dispenser.UserSignatures.Count > 0);

        RuleFor(x => x.Dispenser)
            .SetValidator(refundValidator)
            .When(x => x.IsRefund);

        RuleFor(x => x.Dispenser)
            .SetValidator(assetValidator);
    }
}