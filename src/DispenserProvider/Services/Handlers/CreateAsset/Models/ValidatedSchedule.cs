using Nethereum.Util;
using System.Numerics;
using TokenSchedule.FluentValidation.Models;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models;

public class ValidatedSchedule(Schedule schedule) : IValidatedScheduleItem
{
    public decimal Ratio { get; } = UnitConversion.Convert.FromWei(BigInteger.Parse(schedule.Ratio), 18);
    public DateTime StartDate { get; } = schedule.StartDate;
    public DateTime? FinishDate { get; } = schedule.FinishDate;
}