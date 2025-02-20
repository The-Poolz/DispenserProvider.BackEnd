using Newtonsoft.Json;

namespace DispenserProvider.Extensions.Pagination;

public interface IPaginated
{
    [JsonRequired]
    public int Limit { get; }

    [JsonRequired]
    public int Page { get; }
}