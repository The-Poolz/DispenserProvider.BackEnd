using Moq;
using Xunit;
using MediatR;
using FluentAssertions;
using DispenserProvider.Models;
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
            Environment.SetEnvironmentVariable("PRODUCTION_MODE", "Stage");

            new DispenserProviderLambda().Should().NotBeNull();
        }
    }

    public class RunAsync
    {
        [Fact]
        internal async Task WhenRequestHandledSuccessfully_ShouldReturnExpectedResponse()
        {
            var request = new LambdaRequest
            {
                CreateAssetRequest = new CreateAssetRequest()
            };
            var handlerResponse = new CreateAssetResponse();

            var serviceProvider = new ServiceCollection()
                .AddScoped(_ => {
                    var handler = new Mock<IMediator>();
                    handler.Setup(x => x.Send(It.IsAny<object>(), CancellationToken.None)).ReturnsAsync(handlerResponse);
                    return handler.Object;
                })
                .BuildServiceProvider();

            var lambda = new DispenserProviderLambda(serviceProvider);

            var response = await lambda.RunAsync(request);

            response.Should().BeEquivalentTo(new LambdaResponse(handlerResponse));
        }
    }
}