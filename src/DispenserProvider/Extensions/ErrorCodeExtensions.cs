using FluentValidation;
using System.Reflection;
using FluentValidation.Results;
using DispenserProvider.Attributes;
using System.Runtime.CompilerServices;

namespace DispenserProvider.Extensions;

public static class ErrorCodeExtensions
{
    public static ValidationException ToException(this ErrorCode error, object? customState = null, [CallerMemberName] string propertyName = "")
    {
        var errorMessage = error.ToErrorMessage();
        var failure = new ValidationFailure(propertyName, errorMessage)
        {
            ErrorCode = error.ToErrorCode(),
            CustomState = customState
        };
        return new ValidationException(errorMessage, [failure]);
    }

    public static string ToErrorCode(this ErrorCode error)
    {
        return error.ToString();
    }

    public static string ToErrorMessage(this ErrorCode error)
    {
        return error.GetType().GetCustomAttribute<ErrorAttribute>()!.Message;
    }
}