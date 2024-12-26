using FluentValidation;
using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Validators.AdminRequest
{
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
                .WithMessage((_, pair) =>
                    pair.Item1 == pair.Item2
                        ? $"Duplicate address found: {pair.Item1}"
                        : $"Addresses must be in ascending order. Found '{pair.Item1}' >= '{pair.Item2}'"
                );
        }
        private static IEnumerable<(EthereumAddress, EthereumAddress)> GetZippedPairs(IEnumerable<EthereumAddress> users) =>
            users.Zip(users.Skip(1));
        internal static bool IsSorted((EthereumAddress First, EthereumAddress Second) pair) =>
            string.Compare(pair.First, pair.Second) < 0;
    }
}
