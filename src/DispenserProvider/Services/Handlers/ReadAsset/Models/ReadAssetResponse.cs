using DispenserProvider.Models;

namespace DispenserProvider.Services.Handlers.ReadAsset.Models;

public class ReadAssetResponse(Asset[] assets) : IHandlerResponse
{
    public ReadAssetResponse() : this([]) { }

    public Asset[] Assets { get; } = assets;
}