using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Database;

public interface ITakenTrackManager
{
    public void ProcessTakenTracks(IEnumerable<DispenserDTO> dispensers);
}