namespace DispenserProvider.Services.Handlers.ListOfAssets.Models;

public class ListOfAssetsResponse(Asset[] assets)
{
    public ListOfAssetsResponse() : this([]) { }

    public int Count { get; } = assets.Length;
    public IEnumerable<Asset> Assets { get; } = assets;
}