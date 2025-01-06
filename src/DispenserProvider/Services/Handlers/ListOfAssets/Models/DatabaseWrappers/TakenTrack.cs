using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Validators.GenerateSignature;

namespace DispenserProvider.Services.Handlers.ListOfAssets.Models.DatabaseWrappers;

public sealed class TakenTrack : TakenTrackDTO
{
    public TakenTrack(string errorCode, DispenserDTO dispenser)
    {
        IsRefunded = errorCode == AssetAvailabilityValidator.Refunded;
        Dispenser = dispenser;
        DispenserId = dispenser.Id;
    }
};