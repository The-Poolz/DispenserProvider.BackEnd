using System.Text;
using FluentValidation;
using DispenserProvider.DataBase;
using DispenserProvider.Extensions;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.MessageTemplate.Models.Validators;
using DispenserProvider.Services.Handlers.DeleteAsset.Models;
using DispenserProvider.Services.Handlers.DeleteAsset.Models.DatabaseWrappers;

namespace DispenserProvider.Services.Handlers.DeleteAsset;

public class DeleteAssetHandler(IDbContextFactory<DispenserContext> dispenserContextFactory, IValidator<DeleteValidatorSettings> validator) : IRequestHandler<DeleteAssetRequest, DeleteAssetResponse>
{
    public DeleteAssetResponse Handle(DeleteAssetRequest request)
    {
        validator.ValidateAndThrow(new DeleteValidatorSettings(
            new AdminRequestValidatorSettings(request.Signature, request.Message.Eip712Message),
            request.Message.UsersToValidate
        ));

        MarkAsDeleted(request);

        return new DeleteAssetResponse();
    }

    private void MarkAsDeleted(DeleteAssetRequest request)
    {
        var dispenserContext = dispenserContextFactory.CreateDbContext();
        var dispensers = dispenserContext.Dispenser
            .Where(d => d.DeletionLogSignature == null && request.Message.ToDelete.Select(x => x.Value).Contains(d.Id))
            .ToList();

        if (request.Message.ToDelete.Count != dispensers.Count)
        {
            var errorMessage = new StringBuilder($"The following addresses, specified by ChainId={request.Message.ChainId} and PoolId={request.Message.PoolId}, were not found:")
                .AppendLine()
                .AppendJoin(Environment.NewLine, request.Message.ToDelete
                    .Select(x => x.Key.Address)
                    .Except(dispensers.Select(x => x.UserAddress))
                )
                .ToString();
            throw errorMessage.ToException(ErrorCode.USERS_FOR_DELETE_NOT_FOUND);
        }

        dispenserContext.Logs.Add(new LogWrapper(request.Signature));
        dispensers.ForEach(dispenser => dispenser.DeletionLogSignature = request.Signature);
        dispenserContext.Dispenser.UpdateRange(dispensers);
        dispenserContext.SaveChanges();
    }
}