using Net.Web3.EthereumWallet;
using DispenserProvider.MessageTemplate.Models.Eip712;

namespace DispenserProvider.Services.Validators.AdminRequest.Models;

public interface IValidatedMessage
{
    public AbstractMessage Eip712Message { get; }
    public IEnumerable<EthereumAddress> UsersToValidate { get; }
}