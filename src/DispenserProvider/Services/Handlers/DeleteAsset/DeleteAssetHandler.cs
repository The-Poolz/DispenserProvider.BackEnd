using MediatR;
using FluentValidation;
using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using Net.Utils.ErrorHandler.Extensions;
using DispenserProvider.MessageTemplate.Models.Validators;
using DispenserProvider.Services.Handlers.DeleteAsset.Models;
using DispenserProvider.Services.Handlers.DeleteAsset.Models.DatabaseWrappers;

namespace DispenserProvider.Services.Handlers.DeleteAsset;

public class DeleteAssetHandler(IDbContextFactory<DispenserContext> dispenserContextFactory, IValidator<DeleteValidatorSettings> validator) : IRequestHandler<DeleteAssetRequest, DeleteAssetResponse>
{
    public Task<DeleteAssetResponse> Handle(DeleteAssetRequest request, CancellationToken cancellationToken)
    {
        validator.ValidateAndThrow(new DeleteValidatorSettings(
            new AdminRequestValidatorSettings(request.Signature, request.Message.Eip712Message),
            request.Message.UsersToValidate
        ));

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