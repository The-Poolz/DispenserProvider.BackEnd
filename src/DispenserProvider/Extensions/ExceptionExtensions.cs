using FluentValidation;
using FluentValidation.Results;
using System.Runtime.CompilerServices;

namespace DispenserProvider.Extensions;

public static class ExceptionExtensions
{
    public static ValidationException ToException(this string errorMessage, ErrorCode errorCode, [CallerMemberName] string propertyName = "")
    {
        var error = new ValidationFailure(propertyName, errorMessage)
        {
            ErrorCode = errorCode.ToString()
        };
        return new ValidationException(errorMessage, [error]);
    }
}