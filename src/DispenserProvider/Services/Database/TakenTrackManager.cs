using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Database.Models;
using DispenserProvider.Services.Validators.Signature;

namespace DispenserProvider.Services.Database;

public class TakenTrackManager(IDbContextFactory<DispenserContext> dispenserContextFactory, AssetAvailabilityValidator assetValidator) : ITakenTrackManager
{
    public void ProcessTakenTracks(IEnumerable<DispenserDTO> dispensers)
    {
        var dispenserContext = dispenserContextFactory.CreateDbContext();
        var isTracksAdded = false;

        dispensers.ToList().ForEach(dispenser =>
        {
            if (dispenser.TakenTrack != null) return;

            var validation = assetValidator.Validate(dispenser);
            if (validation.IsValid) return;

            dispenserContext.TakenTrack.Add(new TakenTrack(validation.Errors[0].ErrorCode, dispenser));
            isTracksAdded = true;
        });

        if (isTracksAdded) dispenserContext.SaveChanges();
    }
}