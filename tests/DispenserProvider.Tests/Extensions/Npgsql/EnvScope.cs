namespace DispenserProvider.Tests.Extensions.Npgsql;

public sealed class EnvScope : IDisposable
{
    private readonly Dictionary<string, string?> _prev = new();

    public static EnvScope Apply(IDictionary<string, string?> vars)
    {
        var scope = new EnvScope();
        foreach (var kv in vars)
        {
            scope._prev[kv.Key] = Environment.GetEnvironmentVariable(kv.Key);
            Environment.SetEnvironmentVariable(kv.Key, kv.Value);
        }
        return scope;
    }

    public void Dispose()
    {
        foreach (var kv in _prev)
            Environment.SetEnvironmentVariable(kv.Key, kv.Value);
    }
}