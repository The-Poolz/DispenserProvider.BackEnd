using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;

namespace DispenserProvider.Tests.Mocks.DataBase;

internal class MockDbContextFactory(bool seed = false) : IDbContextFactory<DispenserContext>
{
    internal DispenserContext Current { get; } = MockDispenserContext.Create(seed);

    public DispenserContext CreateDbContext()
    {
        return Current;
    }
}