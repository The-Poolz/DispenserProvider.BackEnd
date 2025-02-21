using FluentValidation;
using DispenserProvider.Services.Validators.Signature;
using DispenserProvider.Services.Handlers.GenerateSignature.Models;

namespace DispenserProvider.Services.Handlers.GenerateSignature;

public class GenerateSignatureValidator : AbstractValidator<GenerateSignatureRequest>
{
    public GenerateSignatureValidator(
        UpdatingSignatureValidator updatingValidator,
        RefundSignatureValidator refundValidator,
        AssetAvailabilityValidator assetValidator
    )
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.ValidatorRequest)
            .SetValidator(updatingValidator)
            .When(x => x.ValidatorRequest.Dispenser.LastUserSignature != null);

        RuleFor(x => x.ValidatorRequest.Dispenser)
            .SetValidator(refundValidator)
            .When(x => x.ValidatorRequest.IsRefund);

        RuleFor(x => x.ValidatorRequest.Dispenser)
            .SetValidator(assetValidator);
    }
}