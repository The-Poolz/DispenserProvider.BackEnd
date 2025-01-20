using Nethereum.Util;
using DispenserProvider.Extensions;
using TokenSchedule.FluentValidation.Models;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models;

public class ValidatedSchedule(Schedule schedule) : IValidatedScheduleItem
{
    public decimal Ratio { get; } = schedule.Ratio.StringRatioToDecimal();
    public DateTime StartDate { get; } = schedule.StartDate;
    public DateTime? FinishDate { get; } = schedule.FinishDate.ToUnixTimestamp() == 0 ? null : schedule.FinishDate;
}