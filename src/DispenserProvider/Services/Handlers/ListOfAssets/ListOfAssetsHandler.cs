using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.Services.Handlers.ListOfAssets.Models;

namespace DispenserProvider.Services.Handlers.ListOfAssets;

public class ListOfAssetsHandler(IDbContextFactory<DispenserContext> dispenserContextFactory) : IRequestHandler<ListOfAssetsRequest, ListOfAssetsResponse>
{
    public ListOfAssetsResponse Handle(ListOfAssetsRequest request)
    {
        using var dispenserContext = dispenserContextFactory.CreateDbContext();
        var assets = dispenserContext.Dispenser
            .Where(x =>
                x.UserAddress == request.UserAddress.Address &&
                x.DeletionLogSignature == null
            )
            .Include(x => x.WithdrawalDetail)
                .ThenInclude(x => x.Builders)
            .Include(x => x.RefundDetail)
                .ThenInclude(x => x!.Builders)
            .Select(dispenser => new Asset(dispenser))
            .ToArray();

        return new ListOfAssetsResponse(assets);
    }
}