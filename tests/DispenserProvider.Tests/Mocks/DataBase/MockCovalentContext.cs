using CovalentDb;
using CovalentDb.Types;
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

    internal static DownloaderSettings DispenserTokensDispensed => new()
    {
        EventHash = "hash",
        Key = "key",
        UrlSet = "url",
        ChainId = 97,
        ResponseType = ResponseType.DispenserTokensDispensed,
        ContractAddress = "0x0000000000000000000000000000000000000001"
    };

    internal static CovalentContext Create()
    {
        var context = new DbContextFactory<CovalentContext>().Create(ContextOption.InMemory, $"{Guid.NewGuid()}");

        var chains = new List<Chain> { Chain }.AsQueryable().BuildMockDbSet();
        var downloaderSettings = new List<DownloaderSettings> { DispenserTokensDispensed }.AsQueryable().BuildMockDbSet();

        context.Chains.AddRange(chains.Object);
        context.DownloaderSettings.AddRange(downloaderSettings.Object);
        context.SaveChanges();

        return context;
    }
}