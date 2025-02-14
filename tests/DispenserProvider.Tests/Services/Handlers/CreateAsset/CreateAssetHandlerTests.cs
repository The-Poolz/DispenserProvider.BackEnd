//using Xunit;
//using System.Net;
//using FluentAssertions;
//using FluentValidation;
//using DispenserProvider.Tests.Mocks.DataBase;
//using DispenserProvider.MessageTemplate.Validators;
//using DispenserProvider.Services.Handlers.CreateAsset;
//using DispenserProvider.Tests.Mocks.Services.Validators;
//using DispenserProvider.Services.Handlers.CreateAsset.Models;
//using DispenserProvider.Services.Validators.AdminRequest;
//using DispenserProvider.Tests.Mocks.Services.Handlers.CreateAsset.Models;

//namespace DispenserProvider.Tests.Services.Handlers.CreateAsset;

//public class CreateAssetHandlerTests
//{
//    public class Handle
//    {
//        [Fact]
//        internal void WhenValidationFailed_ShouldThrowException()
//        {
//            var handler = new CreateAssetHandler(
//                new MockDbContextFactory(),
//                new CreateValidator(
//                    new MockAdminValidationService()
//                ),
//                new PoolOwnershipValidator(
//                    new 
//                )
//            );
//            var request = new CreateAssetRequest
//            {
//                Message = MockCreateAssetRequest.Message,
//                Signature = MockCreateAssetRequest.UnauthorizedUserSignature
//            };

//            var testCode = () => handler.Handle(request);

//            testCode.Should().Throw<ValidationException>()
//                .Which.Errors.Should().ContainSingle()
//                .Which.Should().BeEquivalentTo(new
//                {
//                    ErrorCode = "RECOVERED_ADDRESS_IS_INVALID",
//                    ErrorMessage = "Recovered address is not valid.",
//                    CustomState = new
//                    {
//                        RecoveredAddress = MockUsers.UnauthorizedUser.Address
//                    }
//                });
//        }

//        [Fact]
//        internal void WhenSavingSuccessfully_ShouldContextContainsExpectedEntities()
//        {
//            var dbFactory = new MockDbContextFactory();
//            var handler = new CreateAssetHandler(
//                dbFactory,
//                new CreateValidator(
//                    new MockAdminValidationService()
//                )
//            );
//            var response = handler.Handle(MockCreateAssetRequest.Request);

//            response.StatusCode.Should().Be(HttpStatusCode.OK);
//            dbFactory.Current.Logs.ToArray().Should().ContainSingle(x =>
//                x.Signature == MockDispenserContext.Log.Signature &&
//                x.IsCreation == true
//            );
//            dbFactory.Current.Dispenser.ToArray().Should().ContainSingle(x =>
//                x.Id == MockDispenserContext.Dispenser.Id && 
//                x.UserAddress == MockDispenserContext.Dispenser.UserAddress &&
//                x.RefundFinishTime == MockDispenserContext.Dispenser.RefundFinishTime
//            );
//            dbFactory.Current.TransactionDetails.ToArray().Should().ContainSingle(x =>
//                x.Id == MockDispenserContext.TransactionDetail.Id &&
//                x.PoolId == MockDispenserContext.TransactionDetail.PoolId &&
//                x.ChainId == MockDispenserContext.TransactionDetail.ChainId
//            );
//            dbFactory.Current.Builders.ToArray().Should().HaveCount(2);
//        }
//    }
//}