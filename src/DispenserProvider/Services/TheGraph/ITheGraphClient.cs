using DispenserProvider.Services.TheGraph.Models;

namespace DispenserProvider.Services.TheGraph;

public interface ITheGraphClient
{
    public Task<ICollection<DispenserUpdateParams>> GetDispenserUpdateParamsAsync(long chainId, int page, int limit);
}