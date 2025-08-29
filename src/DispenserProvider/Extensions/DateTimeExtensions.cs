namespace DispenserProvider.Extensions;

public static class DateTimeExtensions
{
    public static DateTime? SpecifyUtcKind(this DateTime? dateTime) => dateTime?.SpecifyUtcKind();
    public static DateTime SpecifyUtcKind(this DateTime dateTime) => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
}