using DispenserProvider.Models;

namespace DispenserProvider.Services.Handlers.ListOfAssets.Models;

public class ListOfAssetsResponse(IEnumerable<Asset> assets) : IHandlerResponse
{
    public ListOfAssetsResponse() : this([]) { }

    public IEnumerable<Asset> Assets { get; } = assets;
}