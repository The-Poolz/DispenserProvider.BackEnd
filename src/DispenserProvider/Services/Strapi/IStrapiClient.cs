using Poolz.Finance.CSharp.Strapi;

namespace DispenserProvider.Services.Strapi;

public interface IStrapiClient
{
    public Chain ReceiveChainInfo(long chainId);
}