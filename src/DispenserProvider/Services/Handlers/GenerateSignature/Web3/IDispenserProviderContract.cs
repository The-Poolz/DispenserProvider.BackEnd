using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Web3;

public interface IDispenserProviderContract
{
    public bool IsTaken(long chainId, long poolId, EthereumAddress address);
}