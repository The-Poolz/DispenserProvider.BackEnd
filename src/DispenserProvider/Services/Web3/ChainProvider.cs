using CovalentDb;
using Nethereum.Web3;
using CovalentDb.Types;
using Net.Web3.EthereumWallet;
using Net.Utils.ErrorHandler.Extensions;

namespace DispenserProvider.Services.Web3;

public class ChainProvider(CovalentContext context) : IChainProvider
{
    private readonly Dictionary<long, EthereumAddress> _contractsAddresses = new()
    {
        { 97, "0xa9c68640C1AA52E91A75F4c5e2786F68049541Ad" }
    };

    public IWeb3 Web3(long chainId)
    {
        var chain = context.Chains.FirstOrDefault(x => x.ChainId == chainId)
            ?? throw ErrorCode.CHAIN_NOT_SUPPORTED.ToException(new
            {
                ChainId = chainId
            });
        return new Nethereum.Web3.Web3(chain.RpcConnection);
    }

    public EthereumAddress DispenserProviderContract(long chainId)
    {
        return _contractsAddresses[chainId];
    }

    public EthereumAddress LockDealNFTContract(long chainId)
    {
        var lockDealNFT = context.DownloaderSettings.FirstOrDefault(x => x.ChainId == chainId && x.ResponseType == ResponseType.LDNFTContractApproved)
           ?? throw ErrorCode.LOCK_DEAL_NFT_NOT_SUPPORTED.ToException(new 
           {
               ChainId = chainId
           });
        return lockDealNFT.ContractAddress;
    }
}