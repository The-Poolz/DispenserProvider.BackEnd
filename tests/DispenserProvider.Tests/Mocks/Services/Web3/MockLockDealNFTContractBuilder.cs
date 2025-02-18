using Moq;
using Net.Web3.EthereumWallet;
using DispenserProvider.Services.Web3.Contracts;

namespace DispenserProvider.Tests.Mocks.Services.Web3;

internal class MockLockDealNFTContractBuilder
{
    private readonly Mock<ILockDealNFTContract> _mock = new();

    public MockLockDealNFTContractBuilder WithOwnerOf(long chainId, long poolId, EthereumAddress returns)
    {
        _mock.Setup(x => x.OwnerOf(chainId, poolId)).Returns(returns);
        return this;
    }

    public MockLockDealNFTContractBuilder WithApprovedContract(long chainId, EthereumAddress address, bool returns)
    {
        _mock.Setup(x => x.ApprovedContract(chainId, address)).Returns(returns);
        return this;
    }

    public ILockDealNFTContract Build() => _mock.Object;
}