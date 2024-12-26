namespace DispenserProvider.Services.Handlers.ReadAsset.Models;

public class Asset(AssetContext assetContext, Dispenser[] dispensers)
{
    public long ChainId { get; } = assetContext.ChainId;
    public long PoolId { get; } = assetContext.PoolId;
    public Dispenser[] Dispensers { get; } = dispensers;
}