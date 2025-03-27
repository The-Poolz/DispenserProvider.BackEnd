using MediatR;
using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.Services.Handlers.CreateAsset.Models;
using DispenserProvider.Services.Handlers.CreateAsset.Models.DatabaseWrappers;

namespace DispenserProvider.Services.Handlers.CreateAsset;

public class CreateAssetHandler(IDbContextFactory<DispenserContext> dispenserContextFactory) : IRequestHandler<CreateAssetRequest, CreateAssetResponse>
{
    public Task<CreateAssetResponse> Handle(CreateAssetRequest request, CancellationToken cancellationToken)
    {
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

            dispenserContext.Dispenser.Add(new DispenserWrapper(request, withdrawalDetails, refundDetails));
        }

        dispenserContext.SaveChanges();
    }

    private TransactionDetailWrapper ProcessTransactionDetail(DispenserContext dispenserContext, User user, CreateAssetMessage message)
    {
        var transactionDetail = new TransactionDetailWrapper(message, user);
        var builders = message.Schedules.Select(x => new BuilderWrapper(user, transactionDetail, x)).ToArray();
        return ProcessTransactionDetail(dispenserContext, transactionDetail, builders);
    }

    private TransactionDetailWrapper ProcessTransactionDetail(DispenserContext dispenserContext, User user, Refund refund)
    {
        var transactionDetail = new TransactionDetailWrapper(refund, user);
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