using DispenserProvider.Services.Strapi;

namespace DispenserProvider.Tests.Mocks.Strapi;

internal class MockStrapiClient(OnChainInfo returns) : IStrapiClient
{
    internal static OnChainInfo DefaultOnChainInfo => new(
        DispenserProvider: "0x0000000000000000000000000000000000000001",
        LockDealNFT: "0x0000000000000000000000000000000000000002",
        MultiCall: "0x0000000000000000000000000000000000000003"
    );

    public OnChainInfo ReceiveChainInfo(long chainId)
    {
        return returns;
    }
}