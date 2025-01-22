using Xunit;
using System.Net;
using FluentAssertions;
using FluentValidation;
using DispenserProvider.DataBase;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.MessageTemplate.Validators;
using DispenserProvider.Services.Handlers.CreateAsset;
using DispenserProvider.Tests.Mocks.Services.Validators;
using DispenserProvider.Services.Handlers.CreateAsset.Models;
using DispenserProvider.Tests.Mocks.Services.Handlers.CreateAsset.Models;

namespace DispenserProvider.Tests.Services.Handlers.CreateAsset;

public class CreateAssetHandlerTests
{
    public class Handle
    {
        private readonly CreateAssetHandler _handler;
        private readonly DispenserContext _dispenserContext;

        public Handle()
        {
            _dispenserContext = MockDispenserContext.Create();
            _handler = new CreateAssetHandler(
                dispenserContext: _dispenserContext,
                requestValidator: new CreateValidator(new MockAdminValidationService())
            );
        }

        [Fact]
        internal void WhenValidationFailed_ShouldThrowException()
        {
            var request = new CreateAssetRequest
            {
                Message = MockCreateAssetRequest.Message,
                Signature = MockCreateAssetRequest.UnauthorizedUserSignature
            };

            var testCode = () => _handler.Handle(request);

            testCode.Should().Throw<ValidationException>()
                .Which.Errors.Should().ContainSingle()
                .Which.ErrorMessage.Should().Be($"Recovered address '{MockUsers.UnauthorizedUser.Address}' is not valid.");
        }

        [Fact]
        internal void WhenSavingSuccessfully_ShouldContextContainsExpectedEntities()
        {
            var response = _handler.Handle(MockCreateAssetRequest.Request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            _dispenserContext.Logs.ToArray().Should().ContainSingle(x =>
                x.Signature == MockDispenserContext.Log.Signature &&
                x.IsCreation == true
            );
            _dispenserContext.Dispenser.ToArray().Should().ContainSingle(x =>
                x.Id == MockDispenserContext.Dispenser.Id && 
                x.UserAddress == MockDispenserContext.Dispenser.UserAddress &&
                x.RefundFinishTime == MockDispenserContext.Dispenser.RefundFinishTime
            );
            _dispenserContext.TransactionDetails.ToArray().Should().ContainSingle(x =>
                x.Id == MockDispenserContext.TransactionDetail.Id &&
                x.PoolId == MockDispenserContext.TransactionDetail.PoolId &&
                x.ChainId == MockDispenserContext.TransactionDetail.ChainId
            );
            _dispenserContext.Builders.ToArray().Should().HaveCount(2);
        }
    }
}