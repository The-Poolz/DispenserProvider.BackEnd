namespace DispenserProvider.Extensions.Pagination;

public static class IQueryableExtensions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, IPaginated paginated, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy)
    {
        return orderBy(query).Skip((paginated.Page - 1) * paginated.Limit).Take(paginated.Limit);
    }
}