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
            .Where(x =>
                x.DeletionLogSignature == null &&
                request.Message.Users.Select(u => u.Address).Contains(x.WithdrawalDetail.UserAddress) &&
                ((x.WithdrawalDetail.ChainId == request.Message.ChainId && x.WithdrawalDetail.PoolId == request.Message.PoolId) ||
                 (x.RefundDetail != null && x.RefundDetail.ChainId == request.Message.ChainId && x.RefundDetail.PoolId == request.Message.PoolId))
            )
            .ToList();

        if (request.Message.Users.Length != dispensers.Count)
        {
            throw ErrorCode.USERS_FOR_DELETE_NOT_FOUND.ToException(customState: new
            {
                Users = request.Message.Users
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