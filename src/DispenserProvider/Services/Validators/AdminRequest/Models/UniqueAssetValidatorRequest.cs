using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Validators.AdminRequest.Models;

public class UniqueAssetValidatorRequest(IEnumerable<EthereumAddress> users, long chainId, long poolId)
{
    public IEnumerable<EthereumAddress> Users { get; } = users;
    public long ChainId { get; } = chainId;
    public long PoolId { get; } = poolId;
}