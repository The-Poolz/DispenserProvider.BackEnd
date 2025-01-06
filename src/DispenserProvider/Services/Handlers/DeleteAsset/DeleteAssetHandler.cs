using System.Text;
using FluentValidation;
using DispenserProvider.DataBase;
using DispenserProvider.Services.Handlers.DeleteAsset.Models;
using DispenserProvider.Services.Validators.AdminRequest.Models;
using DispenserProvider.Services.Handlers.DeleteAsset.Models.DatabaseWrappers;

namespace DispenserProvider.Services.Handlers.DeleteAsset;

public class DeleteAssetHandler(DispenserContext dispenserContext, IValidator<AdminValidationRequest> validator) : IRequestHandler<DeleteAssetRequest, DeleteAssetResponse>
{
    private const string NameOfDispenserRole = "DispenserAdmin";

    public DeleteAssetResponse Handle(DeleteAssetRequest request)
    {
        validator.ValidateAndThrow(new AdminValidationRequest(NameOfDispenserRole, request));

        MarkAsDeleted(request);

        return new DeleteAssetResponse();
    }

    private void MarkAsDeleted(DeleteAssetRequest request)
    {
        var dispensers = dispenserContext.Dispenser
            .Where(d => d.DeletionLogSignature == null && request.Message.ToDelete.Select(x => x.Value).Contains(d.Id))
            .ToList();

        if (request.Message.ToDelete.Count != dispensers.Count)
        {
            throw new InvalidOperationException(
                new StringBuilder($"The following addresses, specified by ChainId={request.Message.ChainId} and PoolId={request.Message.PoolId}, were not found:")
                    .AppendLine()
                    .AppendJoin(Environment.NewLine, request.Message.ToDelete
                        .Select(x => x.Key.Address)
                        .Except(dispensers.Select(x => x.UserAddress))
                    )
                    .ToString()
            );
        }

        dispenserContext.Logs.Add(new LogWrapper(request.Signature));
        dispensers.ForEach(dispenser => dispenser.DeletionLogSignature = request.Signature);
        dispenserContext.Dispenser.UpdateRange(dispensers);
        dispenserContext.SaveChanges();
    }
}