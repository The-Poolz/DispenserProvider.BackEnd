using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Database.Models;

public sealed class TakenTrack : TakenTrackDTO
{
    public TakenTrack(bool isRefunded, DispenserDTO dispenser)
    {
        IsRefunded = isRefunded;
        Dispenser = dispenser;
        DispenserId = dispenser.Id;
    }
};