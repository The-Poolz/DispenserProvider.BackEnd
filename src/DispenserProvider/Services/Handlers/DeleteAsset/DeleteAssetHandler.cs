using MediatR;
using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using Net.Utils.ErrorHandler.Extensions;
using DispenserProvider.Services.Handlers.DeleteAsset.Models;
using DispenserProvider.Services.Handlers.DeleteAsset.Models.DatabaseWrappers;

namespace DispenserProvider.Services.Handlers.DeleteAsset;

public class DeleteAssetHandler(IDbContextFactory<DispenserContext> dispenserContextFactory) : IRequestHandler<DeleteAssetRequest, DeleteAssetResponse>
{
    public Task<DeleteAssetResponse> Handle(DeleteAssetRequest request, CancellationToken cancellationToken)
    {
        MarkAsDeleted(request);

        return Task.FromResult(new DeleteAssetResponse());
    }

    private void MarkAsDeleted(DeleteAssetRequest request)
    {
        var dispenserContext = dispenserContextFactory.CreateDbContext();
        var dispensers = dispenserContext.Dispenser
            .Where(d => d.DeletionLogSignature == null && request.Message.ToDelete.Select(x => x.Value).Contains(d.Id))
            .ToList();

        if (request.Message.ToDelete.Count != dispensers.Count)
        {
            throw ErrorCode.USERS_FOR_DELETE_NOT_FOUND.ToException(customState: new
            {
                Users = request.Message.ToDelete
                    .Select(x => x.Key.Address)
                    .Except(dispensers.Select(x => x.UserAddress))
                    .ToArray()
            });
        }

        dispenserContext.Logs.Add(new LogWrapper(request.Signature));
        dispensers.ForEach(dispenser => dispenser.DeletionLogSignature = request.Signature);
        dispenserContext.Dispenser.UpdateRange(dispensers);
        dispenserContext.SaveChanges();
    }
}