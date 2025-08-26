using Nethereum.Util;
using FluentValidation;
using EnvironmentManager.Extensions;
using DispenserProvider.DataBase.Models;
using Net.Utils.ErrorHandler.Extensions;
using DispenserProvider.Services.Validators.Signature.Models;

namespace DispenserProvider.Services.Validators.Signature;

public class UpdatingSignatureValidator : AbstractValidator<GenerateSignatureValidatorRequest>
{
    public UpdatingSignatureValidator()
    {
        RuleFor(x => x.Dispenser.LastUserSignature!)
            .Cascade(CascadeMode.Stop)
            .Must(x => DateTime.UtcNow >= x.ValidUntil.ToUniversalTime())
            .WithError(ErrorCode.SIGNATURE_IS_STILL_VALID, x => new
            {
                ValidFrom = x.Dispenser.LastUserSignature!.ValidFrom.ToUnixTimestamp(),
                NextTry = NextTry(x.Dispenser).ToUnixTimeSeconds()
            })
            .Must(x => DateTime.UtcNow >= NextTry(x))
            .When(x => x.IsRefund != x.Dispenser.LastUserSignature!.IsRefund)
            .WithError(ErrorCode.SIGNATURE_GENERATION_VALID_TIME_NOT_ARRIVED, x => new
            {
                NextTry = NextTry(x.Dispenser).ToUnixTimeSeconds()
            });
    }

    public static DateTime NextTry(SignatureDTO signature) => signature.ValidUntil.ToUniversalTime() + TimeSpan.FromSeconds(Env.COOLDOWN_OFFSET_IN_SECONDS.GetRequired<int>());
    public static DateTime NextTry(DispenserDTO dispenser) => NextTry(dispenser.LastUserSignature!);
}