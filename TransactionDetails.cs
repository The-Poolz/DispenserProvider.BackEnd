namespace DispenserProvider;

public interface ITransactionDetails
{
    public long ChainId { get;  } 
    public long PoolId { get; } 
    public List<IBuilder> Builders { get; }
}


