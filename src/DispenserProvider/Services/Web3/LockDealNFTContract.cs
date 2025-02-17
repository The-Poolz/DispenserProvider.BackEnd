using System.Numerics;
using Nethereum.ABI.Model;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.ABI.Decoders;
using Net.Web3.EthereumWallet;
using Nethereum.ABI.FunctionEncoding;
using Net.Web3.EthereumWallet.Extensions;

namespace DispenserProvider.Services.Web3;

public class LockDealNFTContract(IChainProvider chainProvider) : ILockDealNFTContract
{
    public EthereumAddress OwnerOf(long chainId, BigInteger tokenId)
    {
        var contractAddress = chainProvider.LockDealNFTContract(chainId);
        var data = new FunctionCallEncoder().EncodeRequest(
            sha3Signature: MethodsSignatures.LockDealNFT.OwnerOf,
            parameters: [new Parameter("uint256", "tokenId")],
            values: [tokenId]
        );
        return RpcCall<AddressTypeDecoder, string>(chainId, contractAddress, data);
    }

    public bool ApprovedContract(long chainId, EthereumAddress address)
    {
        var contractAddress = chainProvider.LockDealNFTContract(chainId);
        var data = new FunctionCallEncoder().EncodeRequest(
            sha3Signature: MethodsSignatures.LockDealNFT.ApprovedContract,
            parameters: [new Parameter("address")],
            values: [address.ConvertToChecksumAddress()]
        );
        return RpcCall<BoolTypeDecoder, bool>(chainId, contractAddress, data);
    }

    private TResponse RpcCall<TDecoder, TResponse>(long chainId, EthereumAddress contractAddress, string encodedData)
        where TDecoder : TypeDecoder, new()
    {
        var web3 = chainProvider.Web3(chainId);
        var rpcResponse = web3.Eth.Transactions.Call.SendRequestAsync(
            callInput: new CallInput(
                data: encodedData,
                addressTo: contractAddress
            )
        ).GetAwaiter().GetResult();
        return new TDecoder().Decode<TResponse>(rpcResponse);
    }
}