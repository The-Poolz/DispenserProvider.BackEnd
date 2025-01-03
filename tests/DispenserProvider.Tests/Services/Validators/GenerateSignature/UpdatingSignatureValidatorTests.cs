using Xunit;
using FluentAssertions;
using FluentValidation;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Validators.Signature;

namespace DispenserProvider.Tests.Services.Validators.GenerateSignature;

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
                .WithMessage($"**Cannot generate signature, because it is still valid until ({signature.ValidUntil}). Please try again after ({UpdatingSignatureValidator.NextTry(signature)}).**");
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
                .WithMessage($"**Cannot generate signature, because the next valid time for generation has not yet arrived. Please try again after ({UpdatingSignatureValidator.NextTry(signature)}).**");
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