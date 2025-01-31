using Nethereum.ABI.EIP712;
using Net.Web3.EthereumWallet;
using DispenserProvider.DataBase.Models;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Web3.Eip712;

[Struct("EIP712Domain")]
public sealed class Eip712Domain : Domain
{
    public Eip712Domain(TransactionDetailDTO transactionDetail, EthereumAddress verifyingContract)
    {
        Name = "DispenserProvider";
        Version = "1";
        ChainId = transactionDetail.ChainId;
        VerifyingContract = verifyingContract;
    }
}