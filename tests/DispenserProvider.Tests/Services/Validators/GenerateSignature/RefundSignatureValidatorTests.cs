using Xunit;
using FluentAssertions;
using FluentValidation;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Validators.GenerateSignature;

namespace DispenserProvider.Tests.Services.Validators.GenerateSignature;

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
                .WithMessage($"**Cannot generate signature for refund, because refund time ({dispenser.RefundFinishTime}) has expired.**");
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