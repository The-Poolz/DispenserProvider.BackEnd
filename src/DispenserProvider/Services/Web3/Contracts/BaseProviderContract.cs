using Net.Web3.EthereumWallet;
using poolz.finance.csharp.contracts.LockDealNFT;

namespace DispenserProvider.Services.Web3.Contracts;

public class BaseProviderContract(IChainProvider chainProvider) : Web3Contract(chainProvider), IBuilderContract
{
    public string Name(long chainId, EthereumAddress address)
    {
        var web3 = ChainProvider.Web3(chainId);
        var contract = new LockDealNFTService(web3, address);
        return contract.NameQueryAsync().GetAwaiter().GetResult();
    }
}