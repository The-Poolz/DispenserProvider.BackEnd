using Net.Web3.EthereumWallet;
using DispenserProvider.Services.Resilience;
using poolz.finance.csharp.contracts.DispenserProvider;

namespace DispenserProvider.Services.Web3.Contracts;

public class DispenserProviderContract(IChainProvider chainProvider, IRetryExecutor retry) : IDispenserProviderContract
{
    public bool IsTaken(long chainId, long poolId, EthereumAddress address)
    {
        var web3 = chainProvider.Web3(chainId);
        var contract = new DispenserProviderService(web3, chainProvider.DispenserProviderContract(chainId));
        return retry.Execute(_ => contract.IsTakenQueryAsync(poolId, address));
    }
}