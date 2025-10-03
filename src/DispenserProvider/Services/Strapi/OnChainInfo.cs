using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Strapi;

public record OnChainInfo(
    EthereumAddress DispenserProvider,
    EthereumAddress LockDealNFT,
    EthereumAddress MultiCall
);