using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.MessageTemplate.Services;

namespace DispenserProvider.Tests.Mocks.Services.Validators;

internal class MockAdminValidationService : IAdminValidationService
{
    internal static string AdminAddress => MockUsers.Admin.Address;

    public bool IsValidAdmin(string userAddress, IReadOnlyCollection<long> chainIDs)
    {
        return userAddress == AdminAddress;
    }
}