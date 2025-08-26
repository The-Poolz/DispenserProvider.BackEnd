using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.DeleteAsset.Models.DatabaseWrappers;

public class LogWrapper : LogDTO
{
    public LogWrapper(string signature)
    {
        Signature = signature;
        CreationTime = DateTime.UtcNow.UtcDateTime;
        IsCreation = false;
    }
}