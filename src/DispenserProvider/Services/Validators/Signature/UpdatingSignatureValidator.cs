using Nethereum.Util;
using FluentValidation;
using EnvironmentManager.Extensions;
using DispenserProvider.DataBase.Models;
using Net.Utils.ErrorHandler.Extensions;

namespace DispenserProvider.Services.Validators.Signature;

public class UpdatingSignatureValidator : AbstractValidator<DispenserDTO>
{
    public UpdatingSignatureValidator()
    {
        RuleFor(x => x.LastUserSignature!)
            .Cascade(CascadeMode.Stop)
            .Must(x => DateTime.UtcNow >= x.ValidUntil)
            .WithError(ErrorCode.SIGNATURE_IS_STILL_VALID, x => new
            {
                ValidFrom = x.LastUserSignature!.ValidFrom.ToUnixTimestamp(),
                NextTry = NextTry(x).ToUnixTimestamp()
            })
            .Must(x => DateTime.UtcNow >= NextTry(x))
            .WithError(ErrorCode.SIGNATURE_GENERATION_VALID_TIME_NOT_ARRIVED, x => new
            {
                NextTry = NextTry(x).ToUnixTimestamp()
            });
    }

    public static DateTime NextTry(SignatureDTO signature) => signature.ValidUntil + TimeSpan.FromSeconds(Env.COOLDOWN_OFFSET_IN_SECONDS.GetRequired<int>());
    private static DateTime NextTry(DispenserDTO dispenser) => NextTry(dispenser.LastUserSignature!);
}