using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Validators.Signature;

namespace DispenserProvider.Services.Database.Models;

public sealed class TakenTrack : TakenTrackDTO
{
    public TakenTrack(string errorCode, DispenserDTO dispenser)
    {
        IsRefunded = errorCode == AssetAvailabilityValidator.ErrorCodeRefunded;
        Dispenser = dispenser;
        DispenserId = dispenser.Id;
    }
};