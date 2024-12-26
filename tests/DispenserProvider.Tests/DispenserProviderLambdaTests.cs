using Moq;
using Xunit;
using FluentAssertions;
using DispenserProvider.Models;
using DispenserProvider.Services;
using Microsoft.Extensions.DependencyInjection;
using DispenserProvider.Services.Handlers.CreateAsset.Models;

namespace DispenserProvider.Tests;

public class DispenserProviderLambdaTests
{
    public class Constructors
    {
        [Fact]
        internal void WithoutParameters()
        {
            new DispenserProviderLambda().Should().NotBeNull();
        }
    }

    public class Run
    {
        [Fact]
        internal void WhenRequestHandledSuccessfully_ShouldReturnExpectedResponse()
        {
            var request = new LambdaRequest
            {
                CreateAssetRequest = new CreateAssetRequest()
            };
            var expectedResponse = new CreateAssetResponse();

            var serviceProvider = new ServiceCollection()
                .AddScoped(_ => {
                    var handler = new Mock<IHandlerFactory>();
                    handler.Setup(x => x.Handle(request)).Returns(expectedResponse);
                    return handler.Object;
                })
                .BuildServiceProvider();

            var lambda = new DispenserProviderLambda(serviceProvider);

            var response = lambda.Run(request);

            response.Should().Be(expectedResponse);
        }
    }
}