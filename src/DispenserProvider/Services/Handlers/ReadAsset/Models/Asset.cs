namespace DispenserProvider.Services.Handlers.ReadAsset.Models;

public class Asset(AssetContext assetContext, IEnumerable<Dispenser> dispensers)
{
    public long ChainId { get; } = assetContext.ChainId;
    public long PoolId { get; } = assetContext.PoolId;
    public IEnumerable<Dispenser> Dispensers { get; } = dispensers;
}