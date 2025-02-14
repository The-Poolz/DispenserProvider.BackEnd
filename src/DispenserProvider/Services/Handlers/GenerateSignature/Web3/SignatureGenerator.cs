using Nethereum.Signer.EIP712;
using DispenserProvider.Services.Web3;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3.Eip712;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Web3;

public class SignatureGenerator(ISignerManager signerManager, IChainProvider chainProvider) : ISignatureGenerator
{
    public string GenerateSignature(TransactionDetailDTO transactionDetail, DateTime validUntil)
    {
        var typedData = new Eip712TypedData(
            transactionDetail,
            chainProvider.DispenserProviderContract(transactionDetail.ChainId),
            validUntil
        );
        return new Eip712TypedDataSigner().SignTypedDataV4(
            json: typedData.ToEip712Json(),
            key: signerManager.GetSigner()
        );
    }
}