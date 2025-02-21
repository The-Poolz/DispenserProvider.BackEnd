namespace DispenserProvider.Services.Handlers.ReadAsset.Models;

public class ReadAssetResponse(IEnumerable<Asset> assets)
{
    public ReadAssetResponse() : this([]) { }

    public IEnumerable<Asset> Assets { get; } = assets;
}