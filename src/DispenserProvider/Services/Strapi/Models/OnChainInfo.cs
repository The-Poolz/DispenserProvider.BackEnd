using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Strapi.Models;

public record OnChainInfo(string RpcUrl, EthereumAddress DispenserProvider, EthereumAddress LockDealNFT);