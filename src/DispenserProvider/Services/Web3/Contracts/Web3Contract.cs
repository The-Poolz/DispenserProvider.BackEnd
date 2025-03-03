using Nethereum.ABI.Decoders;
using Nethereum.RPC.Eth.DTOs;
using Net.Web3.EthereumWallet;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Hex.HexConvertors.Extensions;

namespace DispenserProvider.Services.Web3.Contracts;

public class Web3Contract(IChainProvider chainProvider)
{
    protected readonly IChainProvider ChainProvider = chainProvider;

    protected TResponse CallFunctionWithParameters<TDecoder, TResponse>(long chainId, EthereumAddress contractAddress, string encodedData)
        where TDecoder : TypeDecoder, new()
    {
        return InternalCallFunction<TDecoder, TResponse>(chainId, new CallInput(
            data: encodedData,
            addressTo: contractAddress
        ));
    }

    protected TResponse CallFunction<TDecoder, TResponse>(long chainId, EthereumAddress contractAddress, string sha3Signature)
        where TDecoder : TypeDecoder, new()
    {
        return InternalCallFunction<TDecoder, TResponse>(chainId, new CallInput(
            data: new FunctionCallEncoder().EncodeRequest(sha3Signature),
            addressTo: contractAddress
        ));
    }

    private TResponse InternalCallFunction<TDecoder, TResponse>(long chainId, CallInput callInput)
        where TDecoder : TypeDecoder, new()
    {
        var web3 = ChainProvider.Web3(chainId);
        var rpcResponse = web3.Eth.Transactions.Call.SendRequestAsync(callInput).GetAwaiter().GetResult();
        return new TDecoder().Decode<TResponse>(rpcResponse.HexToByteArray());
    }
}