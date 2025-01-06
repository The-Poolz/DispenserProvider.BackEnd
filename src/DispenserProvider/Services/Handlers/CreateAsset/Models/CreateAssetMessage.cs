using Newtonsoft.Json;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models;

public class CreateAssetMessage
{
    [JsonRequired]
    public Schedule[] Schedules { get; set; } = [];

    [JsonRequired]
    public User[] Users { get; set; } = [];

    public Refund? Refund { get; set; }

    [JsonRequired]
    public long ChainId { get; set; }

    [JsonRequired]
    public long PoolId { get; set; }
}