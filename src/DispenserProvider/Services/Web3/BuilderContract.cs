using Nethereum.ABI.Decoders;
using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Web3;

public class BuilderContract(IChainProvider chainProvider) : Web3Contract(chainProvider), IBuilderContract
{
    public string Name(long chainId, EthereumAddress address)
    {
        return CallFunction<StringTypeDecoder, string>(chainId, address, MethodsSignatures.BaseProvider.Name);
    }
}