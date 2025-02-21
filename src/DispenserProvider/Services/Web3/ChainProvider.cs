using CovalentDb;
using Nethereum.Web3;
using CovalentDb.Types;
using Net.Web3.EthereumWallet;
using Net.Utils.ErrorHandler.Extensions;

namespace DispenserProvider.Services.Web3;

public class ChainProvider(CovalentContext context) : IChainProvider
{
    public IWeb3 Web3(long chainId)
    {
        var chain = context.Chains.FirstOrDefault(x => x.ChainId == chainId)
            ?? throw ErrorCode.CHAIN_NOT_SUPPORTED.ToException(new
            {
                ChainId = chainId
            });
        return new Nethereum.Web3.Web3(chain.RpcConnection);
    }

    public EthereumAddress DispenserProviderContract(long chainId) => GetContract(chainId, ResponseType.DispenserTokensDispensed, ErrorCode.DISPENSER_PROVIDER_NOT_SUPPORTED);

    public EthereumAddress LockDealNFTContract(long chainId) => GetContract(chainId, ResponseType.LDNFTContractApproved, ErrorCode.LOCK_DEAL_NFT_NOT_SUPPORTED);

    private EthereumAddress GetContract(long chainId, ResponseType responseType, ErrorCode error)
    {
        var contractAddress = context.DownloaderSettings.FirstOrDefault(x => x.ChainId == chainId && x.ResponseType == responseType)
            ?? throw error.ToException(new
            {
                ChainId = chainId
            });
        return contractAddress.ContractAddress;
    }
}