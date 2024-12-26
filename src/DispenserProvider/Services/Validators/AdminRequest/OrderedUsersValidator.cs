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
            .Must(users => users.Zip(users.Skip(1))
                .All(pair => string.Compare(pair.First, pair.Second) < 0))
            .WithMessage(FormatMessage);
    }

    private static string FormatMessage(IEnumerable<EthereumAddress> addresses)
    {
        var (First, Second) = addresses.Zip(addresses.Skip(1))
            .FirstOrDefault(pair => string.Compare(pair.First, pair.Second) >= 0);
        return $"Error on {First} and {Second} " + (First == Second ?
            "All addresses must be unique." :
        "All addresses must be in ascending order.");
    }
}
