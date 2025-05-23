﻿using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models.DatabaseWrappers;

public class LogWrapper : LogDTO
{
    public LogWrapper(string signature)
    {
        Signature = signature;
        CreationTime = DateTime.UtcNow;
        IsCreation = true;
    }
}