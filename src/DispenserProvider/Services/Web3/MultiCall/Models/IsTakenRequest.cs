using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Web3.MultiCall.Models;

public record IsTakenRequest(string DispenserId, long PoolId, EthereumAddress Address, bool IsRefund);