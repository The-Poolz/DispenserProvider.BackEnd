using System.Numerics;
using Nethereum.ABI.Model;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.ABI.Decoders;
using Net.Web3.EthereumWallet;
using Nethereum.ABI.FunctionEncoding;

namespace DispenserProvider.Services.Web3;

public class LockDealNFTContract(IChainProvider chainProvider) : ILockDealNFTContract
{
    public EthereumAddress OwnerOf(long chainId, BigInteger tokenId)
    {
        var web3 = chainProvider.Web3(chainId);
        var contractAddress = chainProvider.LockDealNFTContract(chainId);

        var rpcResponse = web3.Eth.Transactions.Call.SendRequestAsync(
            callInput: new CallInput(
                data: new FunctionCallEncoder().EncodeRequest(
                    sha3Signature: MethodsSignatures.LockDealNFT.OwnerOf,
                    parameters: [new Parameter("uint256", "tokenId")],
                    values: [tokenId]
                ),
                addressTo: contractAddress
            )
        ).GetAwaiter().GetResult();

        return new AddressTypeDecoder().Decode<EthereumAddress>(rpcResponse);
    }
}