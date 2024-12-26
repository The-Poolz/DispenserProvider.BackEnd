using FluentValidation;
using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Validators.AdminRequest;

public class OrderedUsersValidator : AbstractValidator<IEnumerable<EthereumAddress>>
{
    public OrderedUsersValidator()
    {
        RuleFor(users => users.ToArray())
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Collection of users cannot be empty.")
            .Must(users => users.Zip(users.Skip(1), (left, right) => (left, right))
                .All(pair => string.Compare(pair.left, pair.right) < 0))
            .WithMessage("All addresses must be unique and in ascending order.");
    }
}
