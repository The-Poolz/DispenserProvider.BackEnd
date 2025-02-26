using Moq;
using Xunit;
using FluentAssertions;
using FluentValidation;
using DispenserProvider.Services.Handlers.CreateAsset;
using DispenserProvider.MessageTemplate.Models.Validators;
using DispenserProvider.Services.Handlers.CreateAsset.Models;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Tests.Services.Handlers.CreateAsset;

public class CreateAssetValidatorTests
{
    public class SameChainValidation
    {
        private readonly CreateAssetValidator _validator = new(
            new Mock<IValidator<CreateValidatorSettings>>().Object,
            new Mock<IValidator<PoolOwnershipValidatorRequest>>().Object,
            new Mock<IValidator<BuildersValidatorRequest>>().Object
        );

        [Fact]
        internal void WhenPoolIdIsDuplicated_ShouldThrowException()
        {
            var request = new CreateAssetRequest {
                Message = new CreateAssetMessage {
                    PoolId = 1,
                    ChainId = 1,
                    Refund = new Refund {
                        PoolId = 1,
                        ChainId = 1
                    }
                }
            };

            var testCode = () => _validator.ValidateAndThrow(request);

            testCode.Should().Throw<ValidationException>()
                .Which.Errors.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    ErrorCode = "POOL_ID_DUPLICATION",
                    ErrorMessage = "PoolId in the specified ChainId is duplicated for Schedule and Refund."
                });
        }

        [Fact]
        internal void WhenPoolIdNotDuplicated_ShouldWithoutException()
        {
            var request = new CreateAssetRequest {
                Message = new CreateAssetMessage {
                    PoolId = 1,
                    ChainId = 1,
                    Refund = new Refund {
                        PoolId = 2,
                        ChainId = 1
                    }
                }
            };

            var testCode = () => _validator.ValidateAndThrow(request);

            testCode.Should().NotThrow<ValidationException>();
        }

        [Fact]
        internal void WhenRefundNotSet_ShouldWithoutException()
        {
            var request = new CreateAssetRequest {
                Message = new CreateAssetMessage {
                    PoolId = 1,
                    ChainId = 1
                }
            };

            var testCode = () => _validator.ValidateAndThrow(request);

            testCode.Should().NotThrow<ValidationException>();
        }
    }
}