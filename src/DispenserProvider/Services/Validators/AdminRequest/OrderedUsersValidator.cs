using FluentValidation;
using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Validators.AdminRequest;

public class OrderedUsersValidator : AbstractValidator<IEnumerable<EthereumAddress>>
{
    public OrderedUsersValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(users => users)
            .NotEmpty()
            .WithMessage("Collection of users cannot be empty.");

        RuleForEach(users => GetZippedPairs(users.ToArray()))
            .Configure(config => config.PropertyName = "OrderCheck")
            .Must(IsSorted)
            .WithMessage(FormatOrderErrorMessage);
    }

    private static IEnumerable<(EthereumAddress, EthereumAddress)> GetZippedPairs(EthereumAddress[] users) =>
        users.Zip(users.Skip(1));

    private static bool IsSorted((EthereumAddress first, EthereumAddress second) pair) =>
        string.Compare(pair.first, pair.second) < 0;

    private static string FormatOrderErrorMessage(IEnumerable<EthereumAddress> _, (EthereumAddress first, EthereumAddress second) pair) =>
        pair.first == pair.second ? $"Duplicate address found: {pair.first}" : $"Addresses must be in ascending order. Found '{pair.first}' > '{pair.second}'";
}
