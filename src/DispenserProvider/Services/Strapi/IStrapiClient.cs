using DispenserProvider.Services.Strapi.Models;

namespace DispenserProvider.Services.Strapi;

public interface IStrapiClient
{
    public OnChainInfo ReceiveChainInfo(long chainId);
}