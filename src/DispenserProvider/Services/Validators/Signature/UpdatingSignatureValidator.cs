using Nethereum.Util;
using FluentValidation;
using DispenserProvider.Extensions;
using EnvironmentManager.Extensions;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Validators.Signature;

public class UpdatingSignatureValidator : AbstractValidator<DispenserDTO>
{
    public UpdatingSignatureValidator()
    {
        RuleFor(x => x.LastUserSignature!)
            .Cascade(CascadeMode.Stop)
            .Must(x => DateTime.UtcNow >= x.ValidUntil)
            .WithState(x => new
            {
                ValidUntil = x.LastUserSignature!.ValidUntil.ToUnixTimestamp(),
                NextTry = NextTry(x).ToUnixTimestamp()
            })
            .WithErrorCode(ErrorCode.SIGNATURE_IS_STILL_VALID.ToErrorCode())
            .WithMessage(ErrorCode.SIGNATURE_IS_STILL_VALID.ToErrorMessage())
            .Must(x => DateTime.UtcNow >= NextTry(x))
            .WithState(x => new
            {
                NextTry = NextTry(x).ToUnixTimestamp()
            })
            .WithErrorCode(ErrorCode.SIGNATURE_GENERATION_VALID_TIME_NOT_ARRIVED.ToErrorCode())
            .WithMessage(ErrorCode.SIGNATURE_GENERATION_VALID_TIME_NOT_ARRIVED.ToErrorMessage());
    }

    public static DateTime NextTry(SignatureDTO signature) => signature.ValidUntil + TimeSpan.FromSeconds(Env.COOLDOWN_OFFSET_IN_SECONDS.GetRequired<int>());
    private static DateTime NextTry(DispenserDTO dispenser) => NextTry(dispenser.LastUserSignature!);
}