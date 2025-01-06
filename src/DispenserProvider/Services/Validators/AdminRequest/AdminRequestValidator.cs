using AuthDB;
using FluentValidation;
using Net.Web3.EthereumWallet;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Services.Validators.AdminRequest;

public class AdminRequestValidator : AbstractValidator<AdminValidationRequest>
{
    public AdminRequestValidator(AuthContext authContext, IValidator<IEnumerable<EthereumAddress>> orderValidator)
    {
        RuleFor(request => request)
            .Must(request => IsValidAdmin(request, authContext))
            .WithMessage(request => $"Recovered address '{request.RecoveredAddress}' is not '{request.NameOfRole}'.")
            .DependentRules(() => {
                RuleFor(request => request.UsersToValidateOrder)
                    .SetValidator(orderValidator);
            });
    }

    private static bool IsValidAdmin(AdminValidationRequest request, AuthContext authContext) => authContext.Users
        .Join(
            authContext.Roles,
            user => user.RoleId,
            role => role.Id,
            (user, role) => new { user, role }
        )
        .Any(x => x.role.Name == request.NameOfRole && x.user.Name == request.RecoveredAddress);
}