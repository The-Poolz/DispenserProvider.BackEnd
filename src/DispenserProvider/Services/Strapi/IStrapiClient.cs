namespace DispenserProvider.Services.Strapi;

public interface IStrapiClient
{
    public OnChainInfo ReceiveChainInfo(long chainId);
    public string ReceiveTheGraphUrl(long chainId);
}