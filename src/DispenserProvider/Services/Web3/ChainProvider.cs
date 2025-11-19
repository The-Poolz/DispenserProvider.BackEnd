using Nethereum.Web3;
using Net.Web3.EthereumWallet;
using Nethereum.JsonRpc.Client;
using Poolz.Finance.CSharp.Http;
using EnvironmentManager.Extensions;
using DispenserProvider.Services.Strapi;

namespace DispenserProvider.Services.Web3;

public class ChainProvider(IStrapiClient strapi) : IChainProvider
{
    private readonly Dictionary<long, OnChainInfo> ChainsInfo = new();

    public string RpcUrl(long chainId) => $"{Env.BASE_URL_OF_RPC.GetRequired()}{chainId}";

    public IWeb3 Web3(long chainId)
    {
        var rpcUrl = RpcUrl(chainId);
        var httpClient = new HttpClientFactory().Create(rpcUrl);
        var web3 = new Nethereum.Web3.Web3(new RpcClient(new Uri(rpcUrl), httpClient));
        return web3;
    }

    public EthereumAddress DispenserProviderContract(long chainId) => FetchChainInfo(chainId).DispenserProvider;

    public EthereumAddress LockDealNFTContract(long chainId) => FetchChainInfo(chainId).LockDealNFT;

    public EthereumAddress MultiCallContract(long chainId) => FetchChainInfo(chainId).MultiCall;

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