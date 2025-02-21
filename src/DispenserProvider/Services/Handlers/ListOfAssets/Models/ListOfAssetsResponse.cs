namespace DispenserProvider.Services.Handlers.ListOfAssets.Models;

public class ListOfAssetsResponse(IEnumerable<Asset> assets)
{
    public ListOfAssetsResponse() : this([]) { }

    public IEnumerable<Asset> Assets { get; } = assets;
}