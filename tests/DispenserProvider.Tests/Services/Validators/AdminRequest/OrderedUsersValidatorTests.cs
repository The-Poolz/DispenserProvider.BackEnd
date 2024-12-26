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
                .WithMessage($"Validation failed: {Environment.NewLine} -- : Addresses must be in ascending order. Found '0x0000000000000000000000000000000000000002' >= '0x0000000000000000000000000000000000000001' Severity: Error");
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
                .WithMessage($"Validation failed: {Environment.NewLine} -- : Duplicate address found: 0x0000000000000000000000000000000000000002 Severity: Error");
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
