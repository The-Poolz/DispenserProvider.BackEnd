using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Helpers;

public interface ISignatureGenerator
{
    public string GenerateSignature(TransactionDetailDTO transactionDetail, DateTime validUntil);
}