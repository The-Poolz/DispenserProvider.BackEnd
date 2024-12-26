using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Validators.GenerateSignature.Models;

public class GenerateSignatureValidatorRequest(DispenserDTO dispenser, bool isRefund)
{
    public DispenserDTO Dispenser { get; } = dispenser;
    public bool IsRefund { get; } = isRefund;
}