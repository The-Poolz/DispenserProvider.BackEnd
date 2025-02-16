using Nethereum.ABI.Model;
using Nethereum.ABI.Decoders;
using Nethereum.RPC.Eth.DTOs;
using Net.Web3.EthereumWallet;
using Nethereum.ABI.FunctionEncoding;

namespace DispenserProvider.Services.Web3;

public class DispenserProviderContract(IChainProvider chainProvider) : IDispenserProviderContract
{
    public bool IsTaken(long chainId, long poolId, EthereumAddress address)
    {
        var data = new FunctionCallEncoder().EncodeRequest(
            sha3Signature: MethodsSignatures.DispenserProvider.IsTaken,
            parameters: [new Parameter("uint256", "poolId"), new Parameter("address", "address")],
            values: [poolId, address.Address]
        );

        var rpcResponse = chainProvider.Web3(chainId).Eth.Transactions.Call.SendRequestAsync(new CallInput(
            data: data,
            addressTo: chainProvider.DispenserProviderContract(chainId)
        )).GetAwaiter().GetResult();

        return new BoolTypeDecoder().Decode<bool>(rpcResponse);
    }
}