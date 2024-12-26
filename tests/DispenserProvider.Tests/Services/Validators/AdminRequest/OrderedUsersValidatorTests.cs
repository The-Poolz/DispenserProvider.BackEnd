using Xunit;
using FluentAssertions;
using FluentValidation;
using Net.Web3.EthereumWallet;
using DispenserProvider.Services.Validators.AdminRequest;

namespace DispenserProvider.Tests.Services.Validators.AdminRequest;

public class OrderedUsersValidatorTests
{
    public class ValidateAndThrow
    {
        private readonly OrderedUsersValidator validator = new();

        [Fact]
        internal void WhenCollectionIsEmpty_ShouldThrowException()
        {
            var users = Array.Empty<EthereumAddress>();

            var testCode = () => validator.ValidateAndThrow(users);

            testCode.Should().Throw<ValidationException>()
                .WithMessage($"Validation failed: {Environment.NewLine} -- : Collection of users cannot be empty. Severity: Error");
        }

        [Fact]
        internal void WhenCollectionNotSorted_ShouldThrowException()
        {
            var users = new EthereumAddress[] {
                "0x0000000000000000000000000000000000000002",
                "0x0000000000000000000000000000000000000001"
            };

            var testCode = () => validator.ValidateAndThrow(users);

            testCode.Should().Throw<ValidationException>()
                .WithMessage($"Validation failed: {Environment.NewLine} -- :* All addresses must be in ascending order. Severity: Error");
        }


        [Fact]
        internal void WhenCollectionNotUniqe_ShouldThrowException()
        {
            var users = new EthereumAddress[] {
                "0x0000000000000000000000000000000000000002",
                "0x0000000000000000000000000000000000000002"
            };

            var testCode = () => validator.ValidateAndThrow(users);

            testCode.Should().Throw<ValidationException>()
                .WithMessage($"Validation failed: {Environment.NewLine} -- :* All addresses must be unique. Severity: Error");
        }

        [Fact]
        internal void WhenCollectionSortedRight_ShouldNotThrowException()
        {
            var users = new EthereumAddress[] {
                "0x0000000000000000000000000000000000000001",
                "0x0000000000000000000000000000000000000002"
            };

            var testCode = () => validator.ValidateAndThrow(users);

            testCode.Should().NotThrow<ValidationException>();
        }

        [Fact]
        internal void WhenCollectionWithOneElement_ShouldNotThrowException()
        {
            var users = new EthereumAddress[] {
                "0x0000000000000000000000000000000000000001"
            };

            var testCode = () => validator.ValidateAndThrow(users);

            testCode.Should().NotThrow<ValidationException>();
        }
    }
}
