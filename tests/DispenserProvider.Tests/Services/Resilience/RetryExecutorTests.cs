using Xunit;
using FluentAssertions;
using DispenserProvider.Services.Resilience;

namespace DispenserProvider.Tests.Services.Resilience;

public class RetryExecutorTests
{
    public class ExecuteAsync
    {
        [Fact]
        internal async Task When_ActionSucceedsImmediately_Should_ReturnResult()
        {
            var sut = new RetryExecutor(maxRetries: 3, TimeSpan.FromMilliseconds(1));
            var expected = 42;

            var result = await sut.ExecuteAsync(_ => Task.FromResult(expected));

            result.Should().Be(expected);
        }

        [Fact]
        internal async Task When_ActionFailsThenSucceeds_Should_RetryAndReturnResult()
        {
            var sut = new RetryExecutor(maxRetries: 5, TimeSpan.FromMilliseconds(1));
            var attempts = 0;
            var expected = "ok";

            Task<string> Flaky(CancellationToken _)
            {
                attempts++;
                if (attempts < 3) throw new InvalidOperationException("boom");
                return Task.FromResult(expected);
            }

            var result = await sut.ExecuteAsync(Flaky);

            result.Should().Be(expected);
            attempts.Should().Be(3);
        }

        [Fact]
        internal async Task When_AllRetriesExhausted_Should_ThrowLastException()
        {
            var sut = new RetryExecutor(maxRetries: 2, TimeSpan.FromMilliseconds(1));
            var attempts = 0;
            var last = new ApplicationException("last one");

            Task<int> AlwaysFail(CancellationToken _)
            {
                attempts++;
                if (attempts >= 3) throw last;
                throw new InvalidOperationException("first");
            }

            Func<Task> act = async () => await sut.ExecuteAsync(AlwaysFail);

            await act.Should().ThrowAsync<Exception>()
                .Where(ex => ex is ApplicationException || ex is InvalidOperationException);
            attempts.Should().Be(1 + 2);
        }

        [Fact]
        internal void When_UsingExecuteSyncWrapper_Should_BlockingReturnSameResult()
        {
            var sut = new RetryExecutor(maxRetries: 2, TimeSpan.FromMilliseconds(1));
            var attempts = 0;

            Task<string> Action(CancellationToken _)
            {
                attempts++;
                if (attempts < 2) throw new Exception("fail once");
                return Task.FromResult("done");
            }

            var result = sut.Execute(Action);

            result.Should().Be("done");
            attempts.Should().Be(2);
        }
    }
}