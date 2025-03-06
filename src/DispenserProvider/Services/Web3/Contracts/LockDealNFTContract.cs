using System.Numerics;
using Net.Web3.EthereumWallet;
using poolz.finance.csharp.contracts.LockDealNFT;

namespace DispenserProvider.Services.Web3.Contracts;

public class LockDealNFTContract(IChainProvider chainProvider) : ILockDealNFTContract
{
    public EthereumAddress OwnerOf(long chainId, BigInteger tokenId)
    {
        var web3 = chainProvider.Web3(chainId);
        var contractService = new LockDealNFTService(web3, chainProvider.LockDealNFTContract(chainId));
        return contractService.OwnerOfQueryAsync(tokenId).GetAwaiter().GetResult();
    }

    public bool ApprovedContract(long chainId, EthereumAddress address)
    {
        var web3 = chainProvider.Web3(chainId);
        var contractService = new LockDealNFTService(web3, chainProvider.LockDealNFTContract(chainId));
        return contractService.ApprovedContractsQueryAsync(address).GetAwaiter().GetResult();
    }
}