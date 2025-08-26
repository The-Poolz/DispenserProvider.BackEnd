using System.Numerics;
using TokenSchedule.FluentValidation.Models;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models;

public class ValidatedSchedule(Schedule schedule) : IValidatedScheduleItem
{
    public BigInteger Ratio { get; } = BigInteger.Parse(schedule.WeiRatio);
    public System.DateTime StartDate { get; } = schedule.StartDate.UtcDateTime;
    public System.DateTime? FinishDate { get; } = schedule.FinishDate.ToUnixTimeSeconds() == 0 ? null : schedule.StartDate.UtcDateTime;
}