using FluentValidation;
using DispenserProvider.DataBase;
using DispenserProvider.MessageTemplate.Models.Validators;
using DispenserProvider.Services.Handlers.CreateAsset.Models;
using DispenserProvider.Services.Handlers.CreateAsset.Models.DatabaseWrappers;

namespace DispenserProvider.Services.Handlers.CreateAsset;

public class CreateAssetHandler(
    DispenserContext dispenserContext,
    IValidator<CreateValidatorSettings> requestValidator
)
    : IRequestHandler<CreateAssetRequest, CreateAssetResponse>
{
    private const string NameOfDispenserRole = "DispenserAdmin";

    public CreateAssetResponse Handle(CreateAssetRequest request)
    {
        requestValidator.ValidateAndThrow(new CreateValidatorSettings(
            new AdminRequestValidatorSettings(
                nameOfRole: NameOfDispenserRole,
                request.Signature,
                request.Message.Eip712Message
            ),
            request.Message.UsersToValidate,
            request.Message.ScheduleToValidate
        ));

        Save(request);

        return new CreateAssetResponse();
    }

    private void Save(CreateAssetRequest request)
    {
        dispenserContext.Logs.Add(new LogWrapper(request.Signature));

        foreach (var user in request.Message.Users)
        {
            var withdrawalDetails = ProcessTransactionDetail(user, request.Message);
            var refundDetails = request.Message.Refund != null ? ProcessTransactionDetail(user, request.Message.Refund) : null;

            dispenserContext.Dispenser.Add(new DispenserWrapper(request, user, withdrawalDetails, refundDetails));
        }

        dispenserContext.SaveChanges();
    }

    private TransactionDetailWrapper ProcessTransactionDetail(User user, CreateAssetMessage message)
    {
        var transactionDetail = new TransactionDetailWrapper(message);
        var builders = message.Schedules.Select(x => new BuilderWrapper(user, transactionDetail, x)).ToArray();
        return ProcessTransactionDetail(transactionDetail, builders);
    }

    private TransactionDetailWrapper ProcessTransactionDetail(User user, Refund refund)
    {
        var transactionDetail = new TransactionDetailWrapper(refund);
        var builders = new[] { new BuilderWrapper(user, transactionDetail, refund) };
        return ProcessTransactionDetail(transactionDetail, builders);
    }

    private TransactionDetailWrapper ProcessTransactionDetail(TransactionDetailWrapper transactionDetail, IEnumerable<BuilderWrapper> builders)
    {
        dispenserContext.TransactionDetails.Add(transactionDetail);
        dispenserContext.Builders.AddRange(builders);
        return transactionDetail;
    }
}