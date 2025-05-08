using Nethereum.Web3;
using Net.Web3.EthereumWallet;
using DispenserProvider.Services.Strapi;

namespace DispenserProvider.Services.Web3;

public class ChainProvider(IStrapiClient strapi) : IChainProvider
{
    private readonly Dictionary<long, OnChainInfo> ChainsInfo = new();

    public IWeb3 Web3(long chainId)
    {
        var chainInfo = FetchChainInfo(chainId);
        return new Nethereum.Web3.Web3(chainInfo.RpcUrl);
    }

    public EthereumAddress DispenserProviderContract(long chainId) => FetchChainInfo(chainId).DispenserProvider;

    public EthereumAddress LockDealNFTContract(long chainId) => FetchChainInfo(chainId).LockDealNFT;

    private OnChainInfo FetchChainInfo(long chainId)
    {
        if (ChainsInfo.TryGetValue(chainId, out var chain))
        {
            return chain;
        }

        var chainInfo = strapi.ReceiveChainInfo(chainId);
        ChainsInfo.Add(chainId, chainInfo);
        return chainInfo;
    }
}