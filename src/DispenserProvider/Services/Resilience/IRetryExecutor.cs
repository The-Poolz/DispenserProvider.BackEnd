namespace DispenserProvider.Services.Resilience;

public interface IRetryExecutor
{
    public T Execute<T>(Func<CancellationToken, Task<T>> action, CancellationToken ct = default);
    public Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken ct = default);
}