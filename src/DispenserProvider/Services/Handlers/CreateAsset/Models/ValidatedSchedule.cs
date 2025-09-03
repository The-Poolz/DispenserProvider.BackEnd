using System.Numerics;
using TokenSchedule.FluentValidation.Models;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models;

public class ValidatedSchedule(Schedule schedule) : IValidatedScheduleItem
{
    public BigInteger Ratio { get; } = BigInteger.Parse(schedule.WeiRatio);
    public DateTime StartDate { get; } = schedule.StartDate.UtcDateTime;
    public DateTime? FinishDate { get; } = schedule.FinishDate.ToUnixTimeSeconds() == 0 ? null : schedule.FinishDate.UtcDateTime;
}