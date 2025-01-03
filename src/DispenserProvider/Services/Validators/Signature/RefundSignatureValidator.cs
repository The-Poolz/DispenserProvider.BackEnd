using FluentValidation;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Validators.Signature;

public class RefundSignatureValidator : AbstractValidator<DispenserDTO>
{
    public RefundSignatureValidator()
    {
        RuleFor(x => x)
            .Must(x => DateTime.UtcNow <= x.RefundFinishTime)
            .WithMessage(x => $"Cannot generate signature for refund, because refund time ({x.RefundFinishTime}) has expired.");
    }
}