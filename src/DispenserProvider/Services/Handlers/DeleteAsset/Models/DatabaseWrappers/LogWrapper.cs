using DispenserProvider.DataBase.Models;
using DispenserProvider.DataBase.Models.Types;

namespace DispenserProvider.Services.Handlers.DeleteAsset.Models.DatabaseWrappers;

public class LogWrapper : LogDTO
{
    public LogWrapper(string signature)
    {
        Signature = signature;
        CreationTime = DateTime.UtcNow;
        Operation = OperationType.Deletion;
    }
}