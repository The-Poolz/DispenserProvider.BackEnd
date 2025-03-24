using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Validators.AdminRequest.Models;

public class UniqueAssetValidatorRequest(EthereumAddress address, long chainId, long poolId)
{
    public string Address { get; } = address;
    public long ChainId { get; } = chainId;
    public long PoolId { get; } = poolId;
}