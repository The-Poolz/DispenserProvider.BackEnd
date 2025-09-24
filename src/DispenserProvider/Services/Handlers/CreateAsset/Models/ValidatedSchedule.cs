using Nethereum.Util;
using System.Numerics;
using TokenSchedule.FluentValidation.Models;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models;

public class ValidatedSchedule(Schedule schedule) : IValidatedScheduleItem
{
    public BigInteger Ratio { get; } = BigInteger.Parse(schedule.WeiRatio);
    public DateTime StartDate { get; } = schedule.StartDate;
    public DateTime? FinishDate { get; } = schedule.FinishDate.ToUnixTimestamp() == 0 ? null : schedule.FinishDate;
}