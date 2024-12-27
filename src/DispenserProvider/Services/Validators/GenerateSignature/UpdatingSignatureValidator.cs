using FluentValidation;
using EnvironmentManager.Extensions;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Validators.GenerateSignature;

public class UpdatingSignatureValidator : AbstractValidator<DispenserDTO>
{
    public UpdatingSignatureValidator()
    {
        RuleFor(x => LastSignature(x))
            .Cascade(CascadeMode.Stop)
            .Must(x => DateTime.UtcNow >= x.ValidUntil)
            .WithMessage(x => $"Cannot generate signature, because it is still valid until ({LastSignature(x).ValidUntil}). Please try again after ({NextTry(x)}).")
            .Must(x => DateTime.UtcNow >= NextTry(x))
            .WithMessage(x => $"Cannot generate signature, because the next valid time for generation has not yet arrived. Please try again after ({NextTry(x)}).");
    }

    public static DateTime NextTry(SignatureDTO signature) => signature.ValidUntil + TimeSpan.FromSeconds(Env.COOLDOWN_OFFSET_IN_SECONDS.GetRequired<int>());
    private static DateTime NextTry(DispenserDTO dispenser) => NextTry(LastSignature(dispenser));
    private static SignatureDTO LastSignature(DispenserDTO dispenser) => dispenser.UserSignatures.MaxBy(s => s.ValidUntil)!;
}