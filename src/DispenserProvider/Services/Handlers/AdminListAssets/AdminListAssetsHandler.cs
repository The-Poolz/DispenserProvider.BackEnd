using MediatR;
using DispenserProvider.Services.TheGraph;
using DispenserProvider.Services.TheGraph.Models;
using DispenserProvider.Services.Handlers.AdminListAssets.Models;

namespace DispenserProvider.Services.Handlers.AdminListAssets;

public class AdminListAssetsHandler(ITheGraphClient theGraph) : IRequestHandler<AdminListAssetsRequest, ICollection<DispenserUpdateParams>>
{
    public Task<ICollection<DispenserUpdateParams>> Handle(AdminListAssetsRequest request, CancellationToken cancellationToken)
    {
        return theGraph.GetDispenserUpdateParamsAsync(request.ChainId, request.Page, request.Limit);
    }
}