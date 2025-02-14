using CovalentDb;
using CovalentDb.Models;
using TestableDbContext.Mock;
using ConfiguredSqlConnection.Extensions;

namespace DispenserProvider.Tests.Mocks.DataBase;

internal static class MockCovalentContext
{
    internal static Chain Chain => new()
    {
        ChainId = 97,
        Name = "BSC Testnet",
        RpcConnection = "http://localhost"
    };

    internal static CovalentContext Create()
    {
        var context = new DbContextFactory<CovalentContext>().Create(ContextOption.InMemory, $"{Guid.NewGuid()}");

        var chains = new List<Chain> { Chain }.AsQueryable().BuildMockDbSet();

        context.Chains.AddRange(chains.Object);
        context.SaveChanges();

        return context;
    }
}