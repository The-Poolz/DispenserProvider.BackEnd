using MediatR;
using Newtonsoft.Json;
using DispenserProvider.Services.TheGraph.Models;

namespace DispenserProvider.Services.Handlers.AdminListAssets.Models;

[method: JsonConstructor]
public class AdminListAssetsRequest(long chainId, int limit = 100, int page = 1) : IRequest<ICollection<DispenserUpdateParams>>
{
    [JsonRequired]
    public long ChainId { get; } = chainId;

    [JsonRequired]
    public int Limit { get; } = limit;

    [JsonRequired]
    public int Page { get; } = page;
}