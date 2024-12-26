using DispenserProvider.DataBase;
using DispenserProvider.DataBase.Models;
using ConfiguredSqlConnection.Extensions;
using DispenserProvider.Tests.Mocks.Services.Handlers.CreateAsset.Models;

namespace DispenserProvider.Tests.Mocks.DataBase;

internal static class MockDispenserContext
{
    internal static DispenserContext Create(bool seed = false)
    {
        var context = new DbContextFactory<DispenserContext>().Create(ContextOption.InMemory, $"{Guid.NewGuid()}");
        if (seed) SeedContext(context);
        return context;
    }

    internal static LogDTO Log => new()
    {
        Signature = MockCreateAssetRequest.Signature,
        CreationTime = DateTime.UtcNow
    };

    internal static DispenserDTO Dispenser => new()
    {
        Id = "00faddb253fe7601b31cd7c7ce9edf73edf47bf58e9e8be72d61964cd14047fe",
        UserAddress = "0x0000000000000000000000000000000000000001",
        RefundFinishTime = DateTimeOffset.FromUnixTimeSeconds(1763544530).DateTime,
        CreationLogSignature = MockCreateAssetRequest.Signature
    };

    internal static TransactionDetailDTO TransactionDetail => new()
    {
        Id = 1,
        PoolId = 1,
        ChainId = 1
    };

    internal static BuilderDTO Builder => new()
    {
        Id = 1,
        WeiAmount = "1465132",
        ProviderAddress = "0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"
    };

    private static void SeedContext(DispenserContext context)
    {
        var log = Log;
        var dispenser = Dispenser;
        var transactionDetail = TransactionDetail;
        var builder = Builder;

        log.CreationDispensers = [dispenser];

        dispenser.CreationLog = log;
        dispenser.CreationLogSignature = log.Signature;
        dispenser.WithdrawalDetail = transactionDetail;
        dispenser.WithdrawalDetailId = transactionDetail.Id;

        transactionDetail.WithdrawalDispenser = dispenser;
        transactionDetail.Builders = [builder];

        builder.TransactionDetail = transactionDetail;
        builder.TransactionDetailId = transactionDetail.Id;

        context.Logs.Add(log);
        context.Dispenser.Add(dispenser);
        context.TransactionDetails.Add(transactionDetail);
        context.Builders.Add(builder);
        context.SaveChanges();
    }
}