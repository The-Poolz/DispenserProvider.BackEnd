using AuthDB;
using EnvironmentManager.Extensions;
using DispenserProvider.MessageTemplate.Services;

namespace DispenserProvider.Services.Validators.AdminRequest;

public class AdminValidationService(AuthContext authContext) : IAdminValidationService
{
    public bool IsValidAdmin(string userAddress)
    {
        return authContext.Users
            .Join(
                authContext.Roles,
                user => user.RoleId,
                role => role.Id,
                (user, role) => new { user, role }
            )
            .Any(x => x.role.Name == Env.NAME_OF_DISPENSER_ADMIN_ROLE.GetRequired<string>(null) && x.user.Name == userAddress);
    }
}