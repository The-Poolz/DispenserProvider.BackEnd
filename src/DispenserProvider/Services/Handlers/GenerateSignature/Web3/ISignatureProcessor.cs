using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Web3;

public interface ISignatureProcessor
{
    public DateTimeOffset SaveSignature(DispenserDTO dispenser, bool isRefund);
}