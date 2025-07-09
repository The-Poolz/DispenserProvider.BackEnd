using MediatR;
using Newtonsoft.Json;
using DispenserProvider.Services.TheGraph.Models;

namespace DispenserProvider.Services.Handlers.AdminListAssets.Models;

[method: JsonConstructor]
public class AdminListAssetsRequest(long chainId) : IRequest<ICollection<DispenserUpdateParams>>
{
    [JsonRequired]
    public long ChainId { get; } = chainId;
}