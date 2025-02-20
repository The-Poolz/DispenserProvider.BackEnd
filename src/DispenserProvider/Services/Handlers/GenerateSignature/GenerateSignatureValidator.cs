using FluentValidation;
using DispenserProvider.Services.Database;
using DispenserProvider.Services.Validators.Signature;
using DispenserProvider.Services.Handlers.GenerateSignature.Models;

namespace DispenserProvider.Services.Handlers.GenerateSignature;

public class GenerateSignatureValidator : AbstractValidator<GenerateSignatureRequest>
{
    public GenerateSignatureValidator(
        IDispenserManager dispenserManager,
        UpdatingSignatureValidator updatingValidator,
        RefundSignatureValidator refundValidator,
        AssetAvailabilityValidator assetValidator
    )
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
    }
}