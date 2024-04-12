namespace DispenserProvider;

public class ResultObject
    (long chainId,
    long poolId,
    string userAddress,
List<IBuilder> builders)
    : ITransactionDetails
{
    public long ChainId { get; private set; } = chainId;
    public long PoolId { get; private set; } = poolId;
    public string UserAddress { get; private set; } = userAddress;
    public List<IBuilder> Builders { get; private set; } = builders;
}

