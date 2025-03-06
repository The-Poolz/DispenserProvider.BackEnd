using Net.Web3.EthereumWallet;
using poolz.finance.csharp.contracts.DispenserProvider;

namespace DispenserProvider.Services.Web3.Contracts;

public class DispenserProviderContract(IChainProvider chainProvider) : IDispenserProviderContract
{
    public bool IsTaken(long chainId, long poolId, EthereumAddress address)
    {
        var web3 = chainProvider.Web3(chainId);
        var contract = new DispenserProviderService(web3, chainProvider.DispenserProviderContract(chainId));
        return contract.IsTakenQueryAsync(poolId, address).GetAwaiter().GetResult();
    }
}