using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Database;

public interface ITakenTrackManager
{
    public IEnumerable<DispenserDTO> ProcessTakenTracks(ICollection<DispenserDTO> dispensers);
}