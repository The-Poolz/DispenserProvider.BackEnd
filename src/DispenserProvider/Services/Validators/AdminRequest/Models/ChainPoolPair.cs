namespace DispenserProvider.Services.Validators.AdminRequest.Models;

public class ChainPoolPair(long chainId, long poolId)
{
    public long ChainId { get; } = chainId;
    public long PoolId { get; } = poolId;
}