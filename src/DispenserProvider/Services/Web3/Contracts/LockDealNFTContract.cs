using System.Numerics;
using Nethereum.ABI.Model;
using Nethereum.ABI.Decoders;
using Net.Web3.EthereumWallet;
using Nethereum.ABI.FunctionEncoding;
using Net.Web3.EthereumWallet.Extensions;

namespace DispenserProvider.Services.Web3.Contracts;

public class LockDealNFTContract(IChainProvider chainProvider) : Web3Contract(chainProvider), ILockDealNFTContract
{
    public EthereumAddress OwnerOf(long chainId, BigInteger tokenId)
    {
        return CallFunctionWithParameters<AddressTypeDecoder, string>(
            chainId,
            ChainProvider.LockDealNFTContract(chainId),
            new FunctionCallEncoder().EncodeRequest(
                sha3Signature: MethodsSignatures.LockDealNFT.OwnerOf,
                parameters: [new Parameter("uint256", "tokenId")],
                values: [tokenId]
            )
        );
    }

    public bool ApprovedContract(long chainId, EthereumAddress address)
    {
        return CallFunctionWithParameters<BoolTypeDecoder, bool>(
            chainId,
            ChainProvider.LockDealNFTContract(chainId),
            new FunctionCallEncoder().EncodeRequest(
                sha3Signature: MethodsSignatures.LockDealNFT.ApprovedContract,
                parameters: [new Parameter("address")],
                values: [address.ConvertToChecksumAddress()]
            )
        );
    }
}