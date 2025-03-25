namespace DispenserProvider.Extensions.Pagination;

public static class PaginationExtensions
{
    public static IEnumerable<T> Paginate<T>(this IEnumerable<T> query, IPaginated paginated, Func<IEnumerable<T>, IOrderedEnumerable<T>> orderBy)
    {
        return orderBy(query).Skip((paginated.Page - 1) * paginated.Limit).Take(paginated.Limit);
    }
}