using Nethereum.Web3;
using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Web3;

public interface IChainProvider
{
    public IWeb3 Web3(long chainId);
    public EthereumAddress ContractAddress(long chainId);
}