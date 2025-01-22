using Xunit;
using System.Net;
using System.Text;
using FluentValidation;
using FluentAssertions;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.MessageTemplate.Validators;
using DispenserProvider.Services.Handlers.DeleteAsset;
using DispenserProvider.Services.Handlers.DeleteAsset.Models;
using DispenserProvider.Tests.Mocks.Services.Handlers.DeleteAsset.Models;

namespace DispenserProvider.Tests.Services.Handlers.DeleteAsset;

public class DeleteAssetHandlerTests
{
    public class Handle
    {
        private readonly DeleteValidator _requestValidator = new(MockAuthContext.Create());

        [Fact]
        internal void WhenValidationFailed_ShouldThrowException()
        {
            var dispenserContext = MockDispenserContext.Create();
            var handler = new DeleteAssetHandler(dispenserContext, _requestValidator);

            var request = new DeleteAssetRequest
            {
                Message = MockDeleteAssetRequest.Message,
                Signature = MockDeleteAssetRequest.UnauthorizedUserSignature
            };

            var testCode = () => handler.Handle(request);

            testCode.Should().Throw<ValidationException>()
                .Which.Errors.Should().ContainSingle()
                .Which.ErrorMessage.Should().Be($"Recovered address '{MockUsers.UnauthorizedUser.Address}' is not '{MockAuthContext.Role.Name}'.");
        }

        [Fact]
        internal void WhenRequestMessageIsInvalid_ShouldThrowException()
        {
            var dispenserContext = MockDispenserContext.Create(seed: true);
            var handler = new DeleteAssetHandler(dispenserContext, _requestValidator);

            var request = new DeleteAssetRequest
            {
                Message = MockDeleteAssetRequest.InvalidMessage,
                Signature = MockDeleteAssetRequest.SignatureForInvalidMessage
            };

            var testCode = () => handler.Handle(request);

            testCode.Should().Throw<InvalidOperationException>()
                .WithMessage(new StringBuilder($"The following addresses, specified by ChainId={request.Message.ChainId} and PoolId={request.Message.PoolId}, were not found:")
                    .AppendLine()
                    .AppendJoin(Environment.NewLine, MockDeleteAssetRequest.InvalidMessage.Users
                        .Select(x => x.Address)
                        .Except(MockDeleteAssetRequest.Message.Users.Select(x => x.Address))
                    )
                    .ToString()
                );
        }

        [Fact]
        internal void WhenMarkedAsDeletedSuccessfully_ShouldContextUpdatedSuccessfully()
        {
            var dispenserContext = MockDispenserContext.Create(seed: true);
            var handler = new DeleteAssetHandler(dispenserContext, _requestValidator);

            var response = handler.Handle(MockDeleteAssetRequest.Request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            dispenserContext.Logs.ToArray().Should().ContainSingle(x =>
                x.Signature == MockDeleteAssetRequest.Request.Signature &&
                x.IsCreation == false
            );
            dispenserContext.Dispenser.ToArray().Should().ContainSingle(x =>
                x.Id == MockDispenserContext.Dispenser.Id &&
                x.UserAddress == MockDispenserContext.Dispenser.UserAddress &&
                x.RefundFinishTime == MockDispenserContext.Dispenser.RefundFinishTime &&
                x.DeletionLogSignature == MockDeleteAssetRequest.Request.Signature
            );
        }
    }
}