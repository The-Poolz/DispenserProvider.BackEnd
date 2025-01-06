using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Database.Models;

namespace DispenserProvider.Services.Database;

public interface IDispenserManager
{
    public DispenserDTO GetDispenser(IGetDispenserRequest request);
}