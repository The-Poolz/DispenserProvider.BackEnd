using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Validators.Signature.Models;

public abstract class SignatureValidatorRequest(DispenserDTO dispenser, bool isRefund)
{
    public DispenserDTO Dispenser { get; } = dispenser;
    public bool IsRefund { get; } = isRefund;
}