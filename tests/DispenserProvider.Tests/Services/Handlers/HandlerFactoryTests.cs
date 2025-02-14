using Moq;
using Xunit;
using FluentAssertions;
using FluentValidation;
using DispenserProvider.Models;
using DispenserProvider.Services;
using DispenserProvider.Extensions;
using DispenserProvider.Services.Handlers;
using Microsoft.Extensions.DependencyInjection;
using DispenserProvider.Services.Handlers.ReadAsset.Models;
using DispenserProvider.Services.Handlers.CreateAsset.Models;
using DispenserProvider.Services.Handlers.DeleteAsset.Models;
using DispenserProvider.Services.Handlers.ListOfAssets.Models;
using DispenserProvider.Services.Handlers.GenerateSignature.Models;
using DispenserProvider.Services.Handlers.RetrieveSignature.Models;
using Net.Utils.ErrorHandler.Extensions;

namespace DispenserProvider.Tests.Services.Handlers;

public class HandlerFactoryTests
{
    public class Handle
    {
        public static IEnumerable<object[]> GetTestData()
        {
            yield return [
                new LambdaRequest { CreateAssetRequest = new CreateAssetRequest() },
                typeof(CreateAssetResponse)
            ];
            yield return [
                new LambdaRequest { DeleteAssetRequest = new DeleteAssetRequest() },
                typeof(DeleteAssetResponse)
            ];
            yield return [
                new LambdaRequest { ReadAssetRequest = new ReadAssetRequest() },
                typeof(ReadAssetResponse)
            ];
            yield return [
                new LambdaRequest { GenerateSignatureRequest = new GenerateSignatureRequest() },
                typeof(GenerateSignatureResponse)
            ];
            yield return [
                new LambdaRequest { RetrieveSignatureRequest = new RetrieveSignatureRequest() },
                typeof(RetrieveSignatureResponse)
            ];
            yield return [
                new LambdaRequest { ListOfAssetsRequest = new ListOfAssetsRequest() },
                typeof(ListOfAssetsResponse)
            ];
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        internal void WhenRequestIsSupported_ShouldReturnsHandlerResponse(LambdaRequest request, Type expectedResponse)
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped(_ => {
                    var handler = new Mock<IRequestHandler<CreateAssetRequest, CreateAssetResponse>>();
                    handler.Setup(x => x.Handle(It.IsAny<CreateAssetRequest>())).Returns(new CreateAssetResponse());
                    return handler.Object;
                })
                .AddScoped(_ => {
                    var handler = new Mock<IRequestHandler<DeleteAssetRequest, DeleteAssetResponse>>();
                    handler.Setup(x => x.Handle(It.IsAny<DeleteAssetRequest>())).Returns(new DeleteAssetResponse());
                    return handler.Object;
                })
                .AddScoped(_ => {
                    var handler = new Mock<IRequestHandler<ReadAssetRequest, ReadAssetResponse>>();
                    handler.Setup(x => x.Handle(It.IsAny<ReadAssetRequest>())).Returns(new ReadAssetResponse());
                    return handler.Object;
                })
                .AddScoped(_ => {
                    var handler = new Mock<IRequestHandler<GenerateSignatureRequest, GenerateSignatureResponse>>();
                    handler.Setup(x => x.Handle(It.IsAny<GenerateSignatureRequest>())).Returns(new GenerateSignatureResponse());
                    return handler.Object;
                })
                .AddScoped(_ => {
                    var handler = new Mock<IRequestHandler<RetrieveSignatureRequest, RetrieveSignatureResponse>>();
                    handler.Setup(x => x.Handle(It.IsAny<RetrieveSignatureRequest>())).Returns(new RetrieveSignatureResponse());
                    return handler.Object;
                })
                .AddScoped(_ => {
                    var handler = new Mock<IRequestHandler<ListOfAssetsRequest, ListOfAssetsResponse>>();
                    handler.Setup(x => x.Handle(It.IsAny<ListOfAssetsRequest>())).Returns(new ListOfAssetsResponse());
                    return handler.Object;
                })
                .BuildServiceProvider();

            var factory = new HandlerFactory(serviceProvider);

            var response = factory.Handle(request);

            response.Should().BeOfType(expectedResponse);
        }

        [Fact]
        internal void WhenRequestNotSupported_ShouldThrowException()
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();
            var factory = new HandlerFactory(serviceProvider);

            var request = new LambdaRequest();

            var testCode = () => factory.Handle(request);

            testCode.Should().Throw<ValidationException>()
                .WithMessage(ErrorCode.INVALID_HANDLER_REQUEST.ToErrorMessage());
        }
    }
}