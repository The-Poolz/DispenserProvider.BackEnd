using Net.Web3.EthereumWallet;
using DispenserProvider.Services.Web3.Contracts.TypeDecoders;

namespace DispenserProvider.Services.Web3.Contracts;

public class BuilderContract(IChainProvider chainProvider) : Web3Contract(chainProvider), IBuilderContract
{
    public string Name(long chainId, EthereumAddress address)
    {
        return CallFunction<StringBytes32TypeDecoder, string>(chainId, address, MethodsSignatures.BaseProvider.Name);
    }
}