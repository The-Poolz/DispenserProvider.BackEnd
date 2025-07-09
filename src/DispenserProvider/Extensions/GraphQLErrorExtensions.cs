using GraphQL;

namespace DispenserProvider.Extensions;

public static class GraphQLErrorExtensions
{
    public static TData EnsureNoErrors<TData>(this GraphQLResponse<TData> response)
    {
        response.Errors = response.AvoidIndexingError();
        if (response.Errors is { Length: > 0 }) throw new InvalidOperationException(
            string.Join(Environment.NewLine, response.Errors.Select(e => e.Message))
        );
        return response.Data;
    }

    internal static GraphQLError[]? AvoidIndexingError<TData>(this GraphQLResponse<TData> response)
    {
        return response.Errors?
            .Where(e => !e.Message.Equals("indexing_error", StringComparison.OrdinalIgnoreCase))
            .ToArray() is { Length: > 0 } filtered ? filtered : null;
    }
}