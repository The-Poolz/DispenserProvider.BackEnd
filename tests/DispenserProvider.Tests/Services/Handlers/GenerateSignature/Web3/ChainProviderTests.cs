using Xunit;
using FluentAssertions;
using DispenserProvider.Services.Web3;
using DispenserProvider.Tests.Mocks.Strapi;

namespace DispenserProvider.Tests.Services.Handlers.GenerateSignature.Web3;

public class ChainProviderTests
{
    public class Web3
    {
        [Fact]
        internal void WhenChainIdIsSupported_ShouldReturnsWeb3()
        {
            Environment.SetEnvironmentVariable("RPC_URL", "http://localhost:5050");

            var strapi = new MockStrapiClient(MockStrapiClient.DefaultOnChainInfo);

            var chainProvider = new ChainProvider(strapi);

            var response = chainProvider.Web3(97);

            response.Should().NotBeNull();
        }
    }

    public class DispenserProviderContract
    {
        [Fact]
        internal void WhenChainIdIsSupported_ShouldReturnsContractAddress()
        {
            var strapi = new MockStrapiClient(MockStrapiClient.DefaultOnChainInfo);

            var chainProvider = new ChainProvider(strapi);

            var response = chainProvider.DispenserProviderContract(97);

            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(MockStrapiClient.DefaultOnChainInfo.DispenserProvider);
        }
    }

    public class LockDealNFTContract
    {
        [Fact]
        internal void WhenChainIdIsSupported_ShouldReturnsContractAddress()
        {
            var strapi = new MockStrapiClient(MockStrapiClient.DefaultOnChainInfo);

            var chainProvider = new ChainProvider(strapi);

            var response = chainProvider.LockDealNFTContract(97);

            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(MockStrapiClient.DefaultOnChainInfo.LockDealNFT);
        }
    }
}