using Nethereum.Util;
using FluentValidation;
using DispenserProvider.Extensions;
using DispenserProvider.Services.Validators.Signature.Models;

namespace DispenserProvider.Services.Validators.Signature;

public class RetrieveSignatureRequestValidator : AbstractValidator<RetrieveSignatureValidatorRequest>
{
    public RetrieveSignatureRequestValidator(AssetAvailabilityValidator assetValidator)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Dispenser.LastUserSignature)
            .NotNull()
            .WithErrorCode(ErrorCode.SIGNATURE_NOT_FOUND.ToErrorCode())
            .WithMessage(ErrorCode.SIGNATURE_NOT_FOUND.ToErrorMessage());

        RuleFor(x => x.Dispenser.LastUserSignature)
            .Must(x => DateTime.UtcNow >= x!.ValidFrom)
            .WithState(x => new
            {
                ValidFrom = x.Dispenser.LastUserSignature!.ValidFrom.ToUnixTimestamp()
            })
            .WithErrorCode(ErrorCode.SIGNATURE_VALID_TIME_NOT_ARRIVED.ToErrorCode())
            .WithMessage(ErrorCode.SIGNATURE_VALID_TIME_NOT_ARRIVED.ToErrorMessage())
            .Must(x => DateTime.UtcNow <= x!.ValidUntil)
            .WithState(x => new
            {
                ValidUntil = x.Dispenser.LastUserSignature!.ValidUntil.ToUnixTimestamp()
            })
            .WithErrorCode(ErrorCode.SIGNATURE_VALID_TIME_IS_EXPIRED.ToErrorCode())
            .WithMessage(ErrorCode.SIGNATURE_VALID_TIME_IS_EXPIRED.ToErrorMessage());

        RuleFor(x => x)
            .Must(x =>
                x.IsRefund == x.Dispenser.LastUserSignature!.IsRefund ||
                !x.IsRefund == !x.Dispenser.LastUserSignature!.IsRefund
            )
            .WithState(x => new
            {
                IsRefund = x.IsRefund == x.Dispenser.LastUserSignature!.IsRefund
            })
            .WithErrorCode(ErrorCode.SIGNATURE_TYPE_IS_INVALID.ToErrorCode())
            .WithMessage(ErrorCode.SIGNATURE_TYPE_IS_INVALID.ToErrorMessage());

        RuleFor(x => x.Dispenser)
            .SetValidator(assetValidator);
    }
}