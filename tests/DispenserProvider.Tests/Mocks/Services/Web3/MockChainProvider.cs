using Nethereum.Web3;
using Net.Web3.EthereumWallet;
using DispenserProvider.Services.Web3;

namespace DispenserProvider.Tests.Mocks.Services.Web3;

internal class MockChainProvider : IChainProvider
{
    public IWeb3 Web3(long chainId)
    {
        throw new NotImplementedException();
    }

    public EthereumAddress DispenserProviderContract(long chainId)
    {
        throw new NotImplementedException();
    }

    public EthereumAddress LockDealNFTContract(long chainId)
    {
        throw new NotImplementedException();
    }
}