namespace Anatini.Server.Utils
{
    public static class DateOnlyNZ
    {
        internal static DateOnly Now
        {
            get {
                var timeZoneInfoNZ = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
                var dateTimeNZ = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfoNZ);
                return DateOnly.FromDateTime(dateTimeNZ);
            }
        }
    }
}
