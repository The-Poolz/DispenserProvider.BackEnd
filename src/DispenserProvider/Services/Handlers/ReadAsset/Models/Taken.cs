using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.ReadAsset.Models;

public class Taken(TakenTrackDTO taken)
{
    public bool IsRefunded { get; } = taken.IsRefunded;
}