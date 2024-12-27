using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Helpers;

public interface ISignatureProcessor
{
    public DateTime SaveSignature(DispenserDTO dispenser, bool isRefund);
}