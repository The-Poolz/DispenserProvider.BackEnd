using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Web3;

public interface ISignatureGenerator
{
    public string GenerateSignature(TransactionDetailDTO transactionDetail, DateTime validUntil);
}