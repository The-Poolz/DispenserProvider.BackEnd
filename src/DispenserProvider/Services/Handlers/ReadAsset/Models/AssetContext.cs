using Newtonsoft.Json;

namespace DispenserProvider.Services.Handlers.ReadAsset.Models;

public class AssetContext
{
    [JsonRequired]
    public long ChainId { get; set; }

    [JsonRequired]
    public long PoolId { get; set; }
}