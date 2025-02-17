using Net.Web3.EthereumWallet;
using DispenserProvider.Services.Web3.Contracts;
using DispenserProvider.Tests.Mocks.Services.Handlers.CreateAsset.Models;

namespace DispenserProvider.Tests.Mocks.Services.Web3;

public class MockBuilderContract(bool isConfigured = false) : IBuilderContract
{
    public string Name(long chainId, EthereumAddress address)
    {
        return isConfigured && (chainId == MockCreateAssetRequest.Message.ChainId || chainId == MockCreateAssetRequest.Message.Refund?.ChainId)
            ? "DealProvider"
            : string.Empty;
    }
}