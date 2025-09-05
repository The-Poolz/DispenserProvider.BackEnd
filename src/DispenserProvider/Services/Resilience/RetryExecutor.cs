using Polly;
using Polly.Retry;

namespace DispenserProvider.Services.Resilience;

public sealed class RetryExecutor(
    int maxRetries = 3,
    TimeSpan? baseDelayInSeconds = null,
    Func<Exception, bool>? shouldRetryOnException = null,
    Action<string>? log = null
) : IRetryExecutor
{
    private readonly TimeSpan _baseDelay = baseDelayInSeconds ?? TimeSpan.FromSeconds(2);
    private readonly Func<Exception, bool> _shouldRetryOnException = shouldRetryOnException ?? (_ => true);

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
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true,
            ShouldHandle = new PredicateBuilder<T>().Handle<Exception>(ex => _shouldRetryOnException(ex)),
            OnRetry = args =>
            {
                var ex = args.Outcome.Exception;
                if (ex != null)
                {
                    log?.Invoke($"[Retry] attempt={args.AttemptNumber}, delay={args.RetryDelay}, exception={ex.GetType().Name}: {ex.Message}");
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