using FluentValidation;
using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Validators.AdminRequest;

public class OrderedUsersValidator : AbstractValidator<IEnumerable<EthereumAddress>>
{
    public OrderedUsersValidator()
    {
        RuleFor(users => users)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Collection of users cannot be empty.");

        RuleForEach(users => GetZippedPairs(users))
            .Configure(config => config.PropertyName = "OrderCheck")
            .Must(IsSorted)
            .WithMessage(MessageFormat);
    }
    private static string MessageFormat(IEnumerable<EthereumAddress> _, (EthereumAddress First, EthereumAddress Second) pair) =>
        pair.First == pair.Second
            ? $"Duplicate address found: {pair.First}"
            : $"Addresses must be in ascending order. Found '{pair.First}' >= '{pair.Second}'";
    private static IEnumerable<(EthereumAddress, EthereumAddress)> GetZippedPairs(IEnumerable<EthereumAddress> users) =>
        users.Zip(users.Skip(1));
    private static bool IsSorted((EthereumAddress First, EthereumAddress Second) pair) =>
        string.Compare(pair.First, pair.Second) < 0;
}
