using AuthDB;
using AuthDB.Models;
using TestableDbContext.Mock;
using Acl.Net.Core.Database.Entities;
using ConfiguredSqlConnection.Extensions;

namespace DispenserProvider.Tests.Mocks.DataBase;

internal static class MockAuthContext
{
    internal static Role Role => new()
    {
        Id = 1,
        Name = "DispenserAdmin"
    };

    internal static AuthorizedUser Admin => new()
    {
        Id = 1,
        Name = MockUsers.Admin.Address,
        RoleId = Role.Id
    };

    internal static AuthContext Create()
    {
        var context = new DbContextFactory<AuthContext>().Create(ContextOption.InMemory, $"{Guid.NewGuid()}");

        var roles = new List<Role> { Role }.AsQueryable().BuildMockDbSet();
        var users = new List<AuthorizedUser> { Admin }.AsQueryable().BuildMockDbSet();

        context.Roles.AddRange(roles.Object);
        context.Users.AddRange(users.Object);
        context.SaveChanges();

        return context;
    }
}