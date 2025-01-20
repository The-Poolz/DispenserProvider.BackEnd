using TokenSchedule.FluentValidation.Models;

namespace DispenserProvider.Services.Validators.AdminRequest.Models;

public interface IValidatedCreateMessage
{
    public IEnumerable<IValidatedScheduleItem> ScheduleToValidate { get; }
}