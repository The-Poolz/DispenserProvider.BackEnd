using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Web3;

public interface ISignatureProcessor
{
    public DateTime SaveSignature(DispenserDTO dispenser, bool isRefund);
}