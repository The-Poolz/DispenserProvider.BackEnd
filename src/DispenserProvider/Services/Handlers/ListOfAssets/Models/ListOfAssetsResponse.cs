namespace DispenserProvider.Services.Handlers.ListOfAssets.Models;

public class ListOfAssetsResponse(int totalAssets, IEnumerable<Asset> assets)
{
    public ListOfAssetsResponse() : this(0, []) { }

    public int TotalAssets { get; } = totalAssets;
    public IEnumerable<Asset> Assets { get; } = assets;
}