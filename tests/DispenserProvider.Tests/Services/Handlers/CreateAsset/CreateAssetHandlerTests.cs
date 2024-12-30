using Xunit;
using System.Net;
using FluentAssertions;
using FluentValidation;
using DispenserProvider.DataBase;
using TokenSchedule.FluentValidation;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Services.Handlers.CreateAsset;
using DispenserProvider.Services.Validators.AdminRequest;
using DispenserProvider.Services.Handlers.CreateAsset.Models;
using DispenserProvider.Tests.Mocks.Services.Handlers.CreateAsset.Models;

namespace DispenserProvider.Tests.Services.Handlers.CreateAsset;

public class CreateAssetHandlerTests
{
    public class Handle
    {
        private readonly CreateAssetHandler handler;
        private readonly DispenserContext dispenserContext;

        public Handle()
        {
            dispenserContext = MockDispenserContext.Create();
            handler = new CreateAssetHandler(
                dispenserContext: dispenserContext,
                requestValidator: new AdminRequestValidator<CreateAssetMessage>(MockAuthContext.Create(), new OrderedUsersValidator()),
                scheduleValidator: new ScheduleValidator()
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

            var testCode = () => handler.Handle(request);

            testCode.Should().Throw<ValidationException>()
                .WithMessage($"Validation failed: {Environment.NewLine} -- : Recovered address '{MockUsers.UnauthorizedUser.Address}' is not '{MockAuthContext.Role.Name}'. Severity: Error");
        }

        [Fact]
        internal void WhenSavingSuccessfully_ShouldContextContainsExpectedEntities()
        {
            var response = handler.Handle(MockCreateAssetRequest.Request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            dispenserContext.Logs.ToArray().Should().ContainSingle(x =>
                x.Signature == MockDispenserContext.Log.Signature &&
                x.IsCreation == true
            );
            dispenserContext.Dispenser.ToArray().Should().ContainSingle(x =>
                x.Id == MockDispenserContext.Dispenser.Id && 
                x.UserAddress == MockDispenserContext.Dispenser.UserAddress &&
                x.RefundFinishTime == MockDispenserContext.Dispenser.RefundFinishTime
            );
            dispenserContext.TransactionDetails.ToArray().Should().ContainSingle(x =>
                x.Id == MockDispenserContext.TransactionDetail.Id &&
                x.PoolId == MockDispenserContext.TransactionDetail.PoolId &&
                x.ChainId == MockDispenserContext.TransactionDetail.ChainId
            );
            dispenserContext.Builders.ToArray().Should().HaveCount(2);
        }
    }
}