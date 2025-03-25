using Net.Web3.EthereumWallet;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Database.Models;

namespace DispenserProvider.Services.Database;

public interface IDispenserManager
{
    public DispenserDTO GetDispenser(IGetDispenserRequest request);
    public IEnumerable<DispenserDTO> GetDispensers(IEnumerable<EthereumAddress> users, long chainId, long poolId);
}