using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Web3;

public interface IBuilderContract
{
    public string Name(long chainId, EthereumAddress address);
}