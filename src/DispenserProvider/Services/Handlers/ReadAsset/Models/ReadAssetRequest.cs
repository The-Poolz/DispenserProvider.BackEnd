using Newtonsoft.Json;
using DispenserProvider.Models;

namespace DispenserProvider.Services.Handlers.ReadAsset.Models;

public class ReadAssetRequest : IHandlerRequest
{
    [JsonRequired]
    public AssetContext[] AssetContext { get; set; } = null!;
}