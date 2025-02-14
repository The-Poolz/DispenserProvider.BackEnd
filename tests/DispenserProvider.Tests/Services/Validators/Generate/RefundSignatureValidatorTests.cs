using Xunit;
using FluentAssertions;
using FluentValidation;
using DispenserProvider.Extensions;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Validators.Signature;
using Net.Utils.ErrorHandler.Extensions;

namespace DispenserProvider.Tests.Services.Validators.Generate;

public class RefundSignatureValidatorTests
{
    public class ValidateAndThrow
    {
        [Fact]
        internal void WhenRefundTimeIsExpired_ShouldThrowException()
        {
            var dispenser = new DispenserDTO {
                RefundFinishTime = DateTime.UtcNow.AddDays(-1)
            };

            var validator = new RefundSignatureValidator();

            var testCode = () => validator.ValidateAndThrow(dispenser);

            testCode.Should().Throw<ValidationException>()
                .WithMessage($"*{ErrorCode.REFUND_TIME_IS_EXPIRED.ToErrorMessage()}*");
        }

        [Fact]
        internal void WhenRefundTimeIsValid_ShouldNotThrowException()
        {
            var dispenser = new DispenserDTO {
                RefundFinishTime = DateTime.UtcNow.AddDays(1)
            };

            var validator = new RefundSignatureValidator();

            var testCode = () => validator.ValidateAndThrow(dispenser);

            testCode.Should().NotThrow();
        }
    }
}