using Nethereum.ABI.Model;
using Nethereum.ABI.Decoders;
using Net.Web3.EthereumWallet;
using Nethereum.ABI.FunctionEncoding;

namespace DispenserProvider.Services.Web3.Contracts;

public class DispenserProviderContract(IChainProvider chainProvider) : Web3Contract(chainProvider), IDispenserProviderContract
{
    public bool IsTaken(long chainId, long poolId, EthereumAddress address)
    {
        return CallFunctionWithParameters<BoolTypeDecoder, bool>(
            chainId,
            ChainProvider.DispenserProviderContract(chainId),
            new FunctionCallEncoder().EncodeRequest(
                sha3Signature: MethodsSignatures.DispenserProvider.IsTaken,
                parameters: [new Parameter("uint256", "poolId"), new Parameter("address", "address")],
                values: [poolId, address.Address]
            )
        );
    }
}