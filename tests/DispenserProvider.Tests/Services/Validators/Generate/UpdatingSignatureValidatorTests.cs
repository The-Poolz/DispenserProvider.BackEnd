using Xunit;
using Nethereum.Util;
using FluentAssertions;
using FluentValidation;
using DispenserProvider.Extensions;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Validators.Signature;
using Net.Utils.ErrorHandler.Extensions;

namespace DispenserProvider.Tests.Services.Validators.Generate;

public class UpdatingSignatureValidatorTests
{
    public class ValidateAndThrow
    {
        public ValidateAndThrow()
        {
            Environment.SetEnvironmentVariable("COOLDOWN_OFFSET_IN_SECONDS", "300");
        }

        [Fact]
        internal void WhenSignatureStillValid_ShouldThrowException()
        {
            var signature = new SignatureDTO {
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidUntil = DateTime.UtcNow.AddDays(1)
            };
            var dispenser = new DispenserDTO {
                UserSignatures = [signature]
            };

            var validator = new UpdatingSignatureValidator();

            var testCode = () => validator.ValidateAndThrow(dispenser);

            testCode.Should().Throw<ValidationException>()
                .WithMessage($"*{ErrorCode.SIGNATURE_IS_STILL_VALID.ToErrorMessage()}*");
        }

        [Fact]
        internal void WhenGenerationOnCooldown_ShouldThrowException()
        {
            var signature = new SignatureDTO{
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidUntil = DateTime.UtcNow
            };
            var dispenser = new DispenserDTO{
                UserSignatures = [signature]
            };

            var validator = new UpdatingSignatureValidator();

            var testCode = () => validator.ValidateAndThrow(dispenser);

            testCode.Should().Throw<ValidationException>()
                .WithMessage($"*{ErrorCode.SIGNATURE_GENERATION_VALID_TIME_NOT_ARRIVED.ToErrorMessage()}*");
        }

        [Fact]
        internal void WhenRequestIsValid_ShouldNotThrowException()
        {
            var signature = new SignatureDTO {
                ValidFrom = DateTime.UtcNow.AddDays(-2),
                ValidUntil = DateTime.UtcNow.AddDays(-1)
            };
            var dispenser = new DispenserDTO {
                UserSignatures = [signature]
            };

            var validator = new UpdatingSignatureValidator();

            var testCode = () => validator.ValidateAndThrow(dispenser);

            testCode.Should().NotThrow();
        }
    }
}