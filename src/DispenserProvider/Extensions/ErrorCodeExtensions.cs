using FluentValidation;
using Net.Utils.ErrorHandler.Extensions;

namespace DispenserProvider.Extensions;

public static class ErrorCodeExtensions
{
    // TODO: Move into `Net.Utils.ErrorHandler` library
    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, ErrorCode error, Func<T, object>? stateProvider = null)
    {
        if (stateProvider != null) rule.WithState(stateProvider);
        return rule
            .WithErrorCode(error)
            .WithErrorMessage(error);
    }
}