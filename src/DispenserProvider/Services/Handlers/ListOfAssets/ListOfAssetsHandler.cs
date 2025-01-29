using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.Services.Validators.Signature;
using DispenserProvider.Services.Handlers.ListOfAssets.Models;
using DispenserProvider.Services.Handlers.ListOfAssets.Models.DatabaseWrappers;

namespace DispenserProvider.Services.Handlers.ListOfAssets;

public class ListOfAssetsHandler(IDbContextFactory<DispenserContext> dispenserContextFactory, AssetAvailabilityValidator assetValidator) : IRequestHandler<ListOfAssetsRequest, ListOfAssetsResponse>
{
    public ListOfAssetsResponse Handle(ListOfAssetsRequest request)
    {
        var dispenserContext = dispenserContextFactory.CreateDbContext();
        var validationResults = dispenserContext.Dispenser
            .Where(x =>
                x.UserAddress == request.UserAddress.Address &&
                x.DeletionLogSignature == null &&
                x.TakenTrack == null
            )
            .Include(x => x.WithdrawalDetail)
                .ThenInclude(x => x.Builders)
            .Include(x => x.RefundDetail)
                .ThenInclude(x => x!.Builders)
            .ToArray()
            .Select(x => new
            {
                Dispenser = x,
                Validation = assetValidator.Validate(x)
            })
            .ToList();

        var isTracksAdded = false;
        validationResults.ForEach(x =>
        {
            if (x.Validation.IsValid) return;
            dispenserContext.TakenTrack.Add(new TakenTrack(x.Validation.Errors[0].ErrorCode, x.Dispenser));
            isTracksAdded = true;
        });
        if (isTracksAdded) dispenserContext.SaveChanges();

        return new ListOfAssetsResponse(validationResults
            .Where(x => x.Validation.IsValid)
            .Select(x => new Asset(x.Dispenser))
            .ToArray()
        );
    }
}