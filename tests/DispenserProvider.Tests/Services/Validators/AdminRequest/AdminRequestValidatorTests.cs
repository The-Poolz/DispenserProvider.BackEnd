using Xunit;
using FluentValidation;
using FluentAssertions;
using Net.Web3.EthereumWallet;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Services.Validators.AdminRequest;
using DispenserProvider.Tests.Mocks.Services.Validators.Models;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Tests.Services.Validators.AdminRequest;

public class AdminRequestValidatorTests
{
    public class ValidateAndThrow
    {
        private readonly AdminRequestValidator<MockPlainMessage> _validator = new(
            MockAuthContext.Create(),
            new OrderedUsersValidator()
        );

        [Fact]
        internal void WhenNameOfRoleNotMatch_ShouldThrowException()
        {
            var adminRequest = MockValidatedAdminRequest.Create();

            var testCode = () => _validator.ValidateAndThrow(
                instance: new AdminValidationRequest<MockPlainMessage>("invalid role name", adminRequest)
            );

            testCode.Should()
                .Throw<ValidationException>()
                .WithMessage($"Validation failed: {Environment.NewLine} -- : Recovered address '{MockUsers.Admin.Address}' is not 'invalid role name'. Severity: Error");
        }

        [Fact]
        internal void WhenAddressNotMatch_ShouldThrowException()
        {
            var adminRequest = MockValidatedAdminRequest.Create(new MockPlainMessage(), MockUsers.UnauthorizedUser.PrivateKey);

            var testCode = () => _validator.ValidateAndThrow(
                instance: new AdminValidationRequest<MockPlainMessage>(MockAuthContext.Role.Name, adminRequest)
            );

            testCode.Should()
                .Throw<ValidationException>()
                .WithMessage($"Validation failed: {Environment.NewLine} -- : Recovered address '{MockUsers.UnauthorizedUser.Address}' is not '{MockAuthContext.Role.Name}'. Severity: Error");
        }

        [Fact]
        internal void WhenCollectionIsEmpty_ShouldThrowException()
        {
            var message = new MockPlainMessage {
                UsersToValidateOrder = Array.Empty<EthereumAddress>()
            };
            var adminRequest = MockValidatedAdminRequest.Create(message, MockUsers.Admin.PrivateKey);

            var testCode = () => _validator.ValidateAndThrow(
                instance: new AdminValidationRequest<MockPlainMessage>(MockAuthContext.Role.Name, adminRequest)
            );

            testCode.Should().Throw<ValidationException>()
                .WithMessage($"Validation failed: {Environment.NewLine} -- UsersToValidateOrder: Collection of users cannot be empty. Severity: Error");
        }

        [Fact]
        internal void WhenCollectionNotSorted_ShouldThrowException()
        {
            var message = new MockPlainMessage
            {
                UsersToValidateOrder = [
                    "0x0000000000000000000000000000000000000002",
                    "0x0000000000000000000000000000000000000001"
                ]
            };
            var adminRequest = MockValidatedAdminRequest.Create(message, MockUsers.Admin.PrivateKey);

            var testCode = () => _validator.ValidateAndThrow(
                instance: new AdminValidationRequest<MockPlainMessage>(MockAuthContext.Role.Name, adminRequest)
            );

            testCode.Should().Throw<ValidationException>()
                .WithMessage($"Validation failed: {Environment.NewLine} -- UsersToValidateOrder.OrderCheck[0]: Addresses must be in ascending order. Found '0x0000000000000000000000000000000000000002' > '0x0000000000000000000000000000000000000001' Severity: Error");
        }

        [Fact]
        internal void WhenRequestIsValid_ShouldWithoutThrownException()
        {
            var adminRequest = MockValidatedAdminRequest.Create();

            var testCode = () => _validator.ValidateAndThrow(
                instance: new AdminValidationRequest<MockPlainMessage>(MockAuthContext.Role.Name, adminRequest)
            );

            testCode.Should().NotThrow<ValidationException>();
        }
    }
}
