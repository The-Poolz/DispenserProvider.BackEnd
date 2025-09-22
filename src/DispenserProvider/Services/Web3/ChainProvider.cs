using System.Net.Http.Headers;
using Nethereum.Web3;
using Net.Web3.EthereumWallet;
using DispenserProvider.Services.Strapi;
using Nethereum.JsonRpc.Client;

namespace DispenserProvider.Services.Web3;

public class ChainProvider(IStrapiClient strapi) : IChainProvider
{
    private readonly Dictionary<long, OnChainInfo> ChainsInfo = new();

    public IWeb3 Web3(long chainId)
    {
        var chainInfo = FetchChainInfo(chainId);
        return new Nethereum.Web3.Web3(
            new RpcClient(
                new Uri(chainInfo.RpcUrl),
                new HttpClient
                {
                    DefaultRequestHeaders =
                    {
                        UserAgent =
                        {
                            new ProductInfoHeaderValue("DispenserLambda", "1.0.0")
                        }
                    }
                }
            )
        );
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