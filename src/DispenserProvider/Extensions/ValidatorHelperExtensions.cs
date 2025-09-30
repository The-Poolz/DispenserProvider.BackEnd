using EnvironmentManager.Extensions;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Extensions;

public static class ValidatorHelperExtensions
{
    public static DateTimeOffset NextTry(this SignatureDTO signature) => signature.ValidUntil + TimeSpan.FromSeconds(Env.COOLDOWN_OFFSET_IN_SECONDS.GetRequired<int>());
    public static DateTimeOffset NextTry(this DispenserDTO dispenser) => NextTry(dispenser.LastUserSignature!);
}