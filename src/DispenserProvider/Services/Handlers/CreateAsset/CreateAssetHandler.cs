using MediatR;
using FluentValidation;
using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.MessageTemplate.Models.Validators;
using DispenserProvider.Services.Handlers.CreateAsset.Models;
using DispenserProvider.Services.Validators.AdminRequest.Models;
using DispenserProvider.Services.Handlers.CreateAsset.Models.DatabaseWrappers;

namespace DispenserProvider.Services.Handlers.CreateAsset;

public class CreateAssetHandler(
    IDbContextFactory<DispenserContext> dispenserContextFactory,
    IValidator<CreateValidatorSettings> requestValidator,
    IValidator<PoolOwnershipValidatorRequest> poolOwnershipValidator,
    IValidator<BuildersValidatorRequest> buildersValidator
)
    : IRequestHandler<CreateAssetRequest, CreateAssetResponse>
{
    public Task<CreateAssetResponse> Handle(CreateAssetRequest request, CancellationToken cancellationToken)
    {
        requestValidator.ValidateAndThrow(new CreateValidatorSettings(
            new AdminRequestValidatorSettings(request.Signature, request.Message.Eip712Message),
            request.Message.UsersToValidate,
            request.Message.ScheduleToValidate
        ));

        poolOwnershipValidator.ValidateAndThrow(new PoolOwnershipValidatorRequest(
            withdraw: new ChainPoolPair(request.Message.ChainId, request.Message.PoolId),
            refund: request.Message.Refund != null ? new ChainPoolPair(request.Message.Refund.ChainId, request.Message.Refund.PoolId) : null
        ));

        buildersValidator.ValidateAndThrow(new BuildersValidatorRequest(
            withdraw: request.Message.Schedules.Select(x => new ChainAddressPair(request.Message.ChainId, x.ProviderAddress)),
            refund: request.Message.Refund != null ? new ChainAddressPair(request.Message.Refund.ChainId, request.Message.Refund.DealProvider) : null
        ));

        Save(request);

        return Task.FromResult(new CreateAssetResponse());
    }

    private void Save(CreateAssetRequest request)
    {
        var dispenserContext = dispenserContextFactory.CreateDbContext();
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