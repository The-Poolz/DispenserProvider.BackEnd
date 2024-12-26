using DispenserProvider.Models;

namespace DispenserProvider.Services.Handlers.ListOfAssets.Models;

public class ListOfAssetsResponse(Asset[] assets) : IHandlerResponse
{
    public ListOfAssetsResponse() : this([]) { }

    public Asset[] Assets { get; } = assets;
}