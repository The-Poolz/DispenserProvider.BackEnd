namespace DispenserProvider.Services.Strapi;

public interface IStrapiClient
{
    public OnChainInfo ReceiveChainInfo(long chainId);
}