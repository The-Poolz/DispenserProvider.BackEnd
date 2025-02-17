using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Web3.Contracts;

public interface IDispenserProviderContract
{
    public bool IsTaken(long chainId, long poolId, EthereumAddress address);
}