using DispenserProvider.Models;

namespace DispenserProvider.Services.Handlers.ReadAsset.Models;

public class ReadAssetResponse(IEnumerable<Asset> assets) : IHandlerResponse
{
    public ReadAssetResponse() : this([]) { }

    public IEnumerable<Asset> Assets { get; } = assets;
}