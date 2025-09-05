using Polly;
using Polly.Retry;
using Amazon.Lambda.Core;

namespace DispenserProvider.Services.Resilience;

public sealed class RetryExecutor(
    int maxRetries = 2,
    TimeSpan? baseDelay = null
) : IRetryExecutor
{
    private readonly TimeSpan _baseDelay = baseDelay ?? TimeSpan.FromMilliseconds(250);

    public T Execute<T>(Func<CancellationToken, Task<T>> action, CancellationToken ct = default)
    {
        return ExecuteAsync(action, ct).GetAwaiter().GetResult();
    }

    public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken ct = default)
    {
        var options = new RetryStrategyOptions<T>
        {
            MaxRetryAttempts = maxRetries,
            Delay = _baseDelay,
            BackoffType = DelayBackoffType.Constant,
            UseJitter = false,
            ShouldHandle = new PredicateBuilder<T>().Handle<Exception>(_ => true),
            OnRetry = args =>
            {
                var ex = args.Outcome.Exception;
                if (ex != null)
                {
                    LambdaLogger.Log($"[Retry] attempt={args.AttemptNumber}, delay={args.RetryDelay}, exception={ex.GetType().Name}: {ex.Message}");
                }
                return default;
            }
        };

        var pipeline = new ResiliencePipelineBuilder<T>()
            .AddRetry(options)
            .Build();

        return await pipeline.ExecuteAsync(token => new ValueTask<T>(action(token)), ct);
    }
}