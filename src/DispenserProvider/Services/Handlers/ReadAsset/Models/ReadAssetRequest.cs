using MediatR;
using Newtonsoft.Json;

namespace DispenserProvider.Services.Handlers.ReadAsset.Models;

public class ReadAssetRequest : IRequest<ReadAssetResponse>
{
    [JsonRequired]
    public AssetContext[] AssetContext { get; set; } = null!;
}