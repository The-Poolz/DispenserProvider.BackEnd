using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Web3.MultiCall.Models;

public record IsTakenResponse(string DispenserId, long PoolId, EthereumAddress Address, bool IsRefund, bool IsTaken);