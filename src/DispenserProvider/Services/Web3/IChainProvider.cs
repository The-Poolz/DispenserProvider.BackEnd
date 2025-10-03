using Nethereum.Web3;
using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Web3;

public interface IChainProvider
{
    public string RpcUrl(long chainId);
    public IWeb3 Web3(long chainId);
    public EthereumAddress DispenserProviderContract(long chainId);
    public EthereumAddress LockDealNFTContract(long chainId);
    public EthereumAddress MultiCallContract(long chainId);
}