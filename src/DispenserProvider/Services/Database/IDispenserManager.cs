using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Database.Models;
using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Database;

public interface IDispenserManager
{
    public DispenserDTO GetDispenser(IGetDispenserRequest request);
    public IEnumerable<DispenserDTO> GetDispensers(EthereumAddress[] users, long chainId, long poolId);
}