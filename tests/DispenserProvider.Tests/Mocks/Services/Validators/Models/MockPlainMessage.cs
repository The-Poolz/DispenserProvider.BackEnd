using Net.Web3.EthereumWallet;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Tests.Mocks.Services.Validators.Models;

internal class MockPlainMessage : IPlainMessage
{
    public string Message => "Hello World!";
    public IEnumerable<EthereumAddress> UsersToValidateOrder { get; set; } = [
        "0x0000000000000000000000000000000000000001",
        "0x0000000000000000000000000000000000000002"
    ];
}