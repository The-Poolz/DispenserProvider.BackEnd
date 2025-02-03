using Nethereum.Util;
using FluentValidation;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Validators.Signature;

public class RefundSignatureValidator : AbstractValidator<DispenserDTO>
{
    public RefundSignatureValidator()
    {
        RuleFor(x => x)
            .Must(x => DateTime.UtcNow <= x.RefundFinishTime)
            .WithState(x => new
            {
                RefundFinishTime = x.RefundFinishTime!.Value.ToUnixTimestamp()
            })
            .WithMessage(x => $"Cannot generate signature for refund, because refund time ({x.RefundFinishTime!.Value.ToUnixTimestamp()}) has expired.");
    }
}