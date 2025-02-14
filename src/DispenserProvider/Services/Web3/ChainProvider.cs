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
        { 1, "0x8080092b3BB79564EF59193f5417421A41E52b99" },
        { 56, "0x8080092b3BB79564EF59193f5417421A41E52b99" },
        { 97, "0x8080092b3BB79564EF59193f5417421A41E52b99" }
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