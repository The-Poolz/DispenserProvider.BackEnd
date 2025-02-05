using Xunit;
using System.Net;
using FluentValidation;
using FluentAssertions;
using DispenserProvider.Extensions;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.MessageTemplate.Validators;
using DispenserProvider.Services.Handlers.DeleteAsset;
using DispenserProvider.Tests.Mocks.Services.Validators;
using DispenserProvider.Services.Handlers.DeleteAsset.Models;
using DispenserProvider.Tests.Mocks.Services.Handlers.DeleteAsset.Models;

namespace DispenserProvider.Tests.Services.Handlers.DeleteAsset;

public class DeleteAssetHandlerTests
{
    public class Handle
    {
        private readonly DeleteValidator _requestValidator = new(new MockAdminValidationService());

        [Fact]
        internal void WhenValidationFailed_ShouldThrowException()
        {
            var dbFactory = new MockDbContextFactory(seed: false);
            var handler = new DeleteAssetHandler(dbFactory, _requestValidator);

            var request = new DeleteAssetRequest
            {
                Message = MockDeleteAssetRequest.Message,
                Signature = MockDeleteAssetRequest.UnauthorizedUserSignature
            };

            var testCode = () => handler.Handle(request);

            testCode.Should().Throw<ValidationException>()
                .Which.Errors.Should().ContainSingle()
                .Which.ErrorMessage.Should().Be($"Recovered address '{MockUsers.UnauthorizedUser.Address}' is not valid.");
        }

        [Fact]
        internal void WhenRequestMessageIsInvalid_ShouldThrowException()
        {
            var dbFactory = new MockDbContextFactory(seed: true);
            var handler = new DeleteAssetHandler(dbFactory, _requestValidator);

            var request = new DeleteAssetRequest
            {
                Message = MockDeleteAssetRequest.InvalidMessage,
                Signature = MockDeleteAssetRequest.SignatureForInvalidMessage
            };

            var testCode = () => handler.Handle(request);

            testCode.Should().Throw<ValidationException>()
                .WithMessage(ErrorCode.USERS_FOR_DELETE_NOT_FOUND.ToErrorMessage());
        }

        [Fact]
        internal void WhenMarkedAsDeletedSuccessfully_ShouldContextUpdatedSuccessfully()
        {
            var dbFactory = new MockDbContextFactory(seed: true);
            var handler = new DeleteAssetHandler(dbFactory, _requestValidator);

            var response = handler.Handle(MockDeleteAssetRequest.Request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            dbFactory.Current.Logs.ToArray().Should().ContainSingle(x =>
                x.Signature == MockDeleteAssetRequest.Request.Signature &&
                x.IsCreation == false
            );
            dbFactory.Current.Dispenser.ToArray().Should().ContainSingle(x =>
                x.Id == MockDispenserContext.Dispenser.Id &&
                x.UserAddress == MockDispenserContext.Dispenser.UserAddress &&
                x.RefundFinishTime == MockDispenserContext.Dispenser.RefundFinishTime &&
                x.DeletionLogSignature == MockDeleteAssetRequest.Request.Signature
            );
        }
    }
}