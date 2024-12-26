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
            .WithMessage("Collection of users cannot be empty.")
            .Must(AllZippedAreSorted)
            .WithMessage(FormatMessage);
    }
    private static string FormatMessage(IEnumerable<EthereumAddress> addresses)
    {
        var miss = Zipped(addresses).FirstOrDefault(IsNotSorted);
        return miss.Item1 == miss.Item2 ?
           $"Duplicate address found: {miss.Item1}" :
        $"Addresses must be in ascending order. Found '{miss.Item1}' >= '{miss.Item2}'";
    }
    internal static IEnumerable<(EthereumAddress,EthereumAddress)> Zipped(IEnumerable<EthereumAddress> users) =>
        users.Zip(users.Skip(1));
    internal static bool IsSorted((EthereumAddress First, EthereumAddress Second) tuple) =>
        string.Compare(tuple.First, tuple.Second) < 0;
    internal static bool IsNotSorted((EthereumAddress First, EthereumAddress Second) tuple) =>
        !IsSorted(tuple);
    internal static bool AllZippedAreSorted(IEnumerable<EthereumAddress> users) =>
        Zipped(users).All(IsSorted);
}

