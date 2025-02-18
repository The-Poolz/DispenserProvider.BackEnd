using Xunit;
using Nethereum.Util;
using FluentAssertions;
using FluentValidation;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Validators.Signature;
using DispenserProvider.Services.Validators.Signature.Models;

namespace DispenserProvider.Tests.Services.Validators.Generate;

public class UpdatingSignatureValidatorTests
{
    public class ValidateAndThrow
    {
        [Fact]
        internal void WhenSignatureStillValid_ShouldThrowException()
        {
            Environment.SetEnvironmentVariable("COOLDOWN_OFFSET_IN_SECONDS", "300");
            var signature = new SignatureDTO {
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidUntil = DateTime.UtcNow.AddDays(1),
                IsRefund = true
            };
            var dispenser = new DispenserDTO {
                UserSignatures = [signature]
            };
            var request = new GenerateSignatureValidatorRequest(dispenser, false);

            var validator = new UpdatingSignatureValidator();

            var testCode = () => validator.ValidateAndThrow(request);

            testCode.Should().Throw<ValidationException>()
                .Which.Errors.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    ErrorCode = "SIGNATURE_IS_STILL_VALID",
                    ErrorMessage = "Cannot generate signature, because it is still valid until.",
                    CustomState = new {
                        ValidFrom = dispenser.LastUserSignature!.ValidFrom.ToUnixTimestamp(),
                        NextTry = UpdatingSignatureValidator.NextTry(dispenser).ToUnixTimestamp()
                    }
                });
        }

        [Fact]
        internal void WhenGenerationOnCooldown_ShouldThrowException()
        {
            Environment.SetEnvironmentVariable("COOLDOWN_OFFSET_IN_SECONDS", "300");
            var signature = new SignatureDTO{
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidUntil = DateTime.UtcNow,
                IsRefund = true
            };
            var dispenser = new DispenserDTO{
                UserSignatures = [signature]
            };
            var request = new GenerateSignatureValidatorRequest(dispenser, false);

            var validator = new UpdatingSignatureValidator();

            var testCode = () => validator.ValidateAndThrow(request);

            testCode.Should().Throw<ValidationException>()
                .Which.Errors.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    ErrorCode = "SIGNATURE_GENERATION_VALID_TIME_NOT_ARRIVED",
                    ErrorMessage = "Cannot generate signature, because the next valid time for generation has not yet arrived.",
                    CustomState = new
                    {
                        NextTry = UpdatingSignatureValidator.NextTry(dispenser).ToUnixTimestamp()
                    }
                });
        }

        [Fact]
        internal void WhenCooldownCheckSkipped_ShouldThrowException()
        {
            // (double check) If cooldown check not will be skipped will receive error cause required env variable not set.
            Environment.SetEnvironmentVariable("COOLDOWN_OFFSET_IN_SECONDS", "");

            var signature = new SignatureDTO
            {
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidUntil = DateTime.UtcNow,
                IsRefund = true
            };
            var dispenser = new DispenserDTO
            {
                UserSignatures = [signature]
            };
            var request = new GenerateSignatureValidatorRequest(dispenser, true);

            var validator = new UpdatingSignatureValidator();

            var testCode = () => validator.ValidateAndThrow(request);

            testCode.Should().NotThrow();
        }

        [Fact]
        internal void WhenRequestIsValid_ShouldNotThrowException()
        {
            Environment.SetEnvironmentVariable("COOLDOWN_OFFSET_IN_SECONDS", "300");
            var signature = new SignatureDTO {
                ValidFrom = DateTime.UtcNow.AddDays(-2),
                ValidUntil = DateTime.UtcNow.AddDays(-1),
                IsRefund = true
            };
            var dispenser = new DispenserDTO {
                UserSignatures = [signature]
            };
            var request = new GenerateSignatureValidatorRequest(dispenser, false);

            var validator = new UpdatingSignatureValidator();

            var testCode = () => validator.ValidateAndThrow(request);

            testCode.Should().NotThrow();
        }
    }
}