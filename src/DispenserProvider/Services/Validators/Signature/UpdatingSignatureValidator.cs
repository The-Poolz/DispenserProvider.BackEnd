using Nethereum.Util;
using FluentValidation;
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
            .WithMessage(x => $"Cannot generate signature, because it is still valid until ({x.LastUserSignature!.ValidUntil.ToUnixTimestamp()}). Please try again after ({NextTry(x).ToUnixTimestamp()}).")
            .Must(x => DateTime.UtcNow >= NextTry(x))
            .WithState(x => new
            {
                NextTry = NextTry(x).ToUnixTimestamp()
            })
            .WithMessage(x => $"Cannot generate signature, because the next valid time for generation has not yet arrived. Please try again after ({NextTry(x).ToUnixTimestamp()}).");
    }

    public static DateTime NextTry(SignatureDTO signature) => signature.ValidUntil + TimeSpan.FromSeconds(Env.COOLDOWN_OFFSET_IN_SECONDS.GetRequired<int>());
    private static DateTime NextTry(DispenserDTO dispenser) => NextTry(dispenser.LastUserSignature!);
}