using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Database.Models;
using DispenserProvider.Services.Validators.Signature;

namespace DispenserProvider.Services.Database;

public class TakenTrackManager(IDbContextFactory<DispenserContext> dispenserContextFactory, AssetAvailabilityValidator assetValidator) : ITakenTrackManager
{
    public IEnumerable<DispenserDTO> ProcessTakenTracks(IEnumerable<DispenserDTO> dispensers)
    {
        var dispenserContext = dispenserContextFactory.CreateDbContext();
        var processed = new List<DispenserDTO>();

        dispensers.ToList().ForEach(dispenser =>
        {
            if (dispenser.TakenTrack != null) return;

            var validation = assetValidator.Validate(dispenser);
            if (validation.IsValid) return;

            dispenserContext.TakenTrack.Add(new TakenTrack(validation.Errors[0].ErrorCode, dispenser));
            processed.Add(dispenser);
        });

        if (processed.Count > 0) dispenserContext.SaveChanges();

        return processed;
    }
}