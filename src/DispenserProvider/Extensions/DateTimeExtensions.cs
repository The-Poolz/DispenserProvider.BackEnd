namespace DispenserProvider.Extensions;

public static class DateTimeExtensions
{
    public static DateTime RoundDateTime(this DateTime dateTime) => new DateTime(
        dateTime.Year, dateTime.Month, dateTime.Day,
        dateTime.Hour, dateTime.Minute, dateTime.Second, DateTimeKind.Utc
    );
    public static DateTime? SpecifyUtcKind(this DateTime? dateTime) => dateTime?.SpecifyUtcKind();
    public static DateTime SpecifyUtcKind(this DateTime dateTime) => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
}