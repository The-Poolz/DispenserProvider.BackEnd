using FluentValidation;
using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.MessageTemplate.Models.Validators;
using DispenserProvider.Services.Handlers.CreateAsset.Models;
using DispenserProvider.Services.Handlers.CreateAsset.Models.DatabaseWrappers;

namespace DispenserProvider.Services.Handlers.CreateAsset;

public class CreateAssetHandler(IDbContextFactory<DispenserContext> dispenserContextFactory, IValidator<CreateValidatorSettings> requestValidator) : IRequestHandler<CreateAssetRequest, CreateAssetResponse>
{
    public CreateAssetResponse Handle(CreateAssetRequest request)
    {
        requestValidator.ValidateAndThrow(new CreateValidatorSettings(
            new AdminRequestValidatorSettings(request.Signature, request.Message.Eip712Message),
            request.Message.UsersToValidate,
            request.Message.ScheduleToValidate
        ));

        Save(request);

        return new CreateAssetResponse();
    }

    private void Save(CreateAssetRequest request)
    {
        using var dispenserContext = dispenserContextFactory.CreateDbContext();
        dispenserContext.Logs.Add(new LogWrapper(request.Signature));

        foreach (var user in request.Message.Users)
        {
            var withdrawalDetails = ProcessTransactionDetail(dispenserContext, user, request.Message);
            var refundDetails = request.Message.Refund != null ? ProcessTransactionDetail(dispenserContext, user, request.Message.Refund) : null;

            dispenserContext.Dispenser.Add(new DispenserWrapper(request, user, withdrawalDetails, refundDetails));
        }

        dispenserContext.SaveChanges();
    }

    private TransactionDetailWrapper ProcessTransactionDetail(DispenserContext dispenserContext, User user, CreateAssetMessage message)
    {
        var transactionDetail = new TransactionDetailWrapper(message);
        var builders = message.Schedules.Select(x => new BuilderWrapper(user, transactionDetail, x)).ToArray();
        return ProcessTransactionDetail(dispenserContext, transactionDetail, builders);
    }

    private TransactionDetailWrapper ProcessTransactionDetail(DispenserContext dispenserContext, User user, Refund refund)
    {
        var transactionDetail = new TransactionDetailWrapper(refund);
        var builders = new[] { new BuilderWrapper(user, transactionDetail, refund) };
        return ProcessTransactionDetail(dispenserContext, transactionDetail, builders);
    }

    private TransactionDetailWrapper ProcessTransactionDetail(DispenserContext dispenserContext, TransactionDetailWrapper transactionDetail, IEnumerable<BuilderWrapper> builders)
    {
        dispenserContext.TransactionDetails.Add(transactionDetail);
        dispenserContext.Builders.AddRange(builders);
        return transactionDetail;
    }
}