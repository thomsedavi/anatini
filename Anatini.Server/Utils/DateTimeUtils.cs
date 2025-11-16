using System.Globalization;

namespace Anatini.Server.Utils
{
    public static class DateTimeUtils
    {
        private static readonly DateTimeFormatInfo info = CultureInfo.GetCultureInfo("en-NZ").DateTimeFormat;
        private static readonly Calendar calendar = info.Calendar;

        public static DateTime GetAccessTokenExpiry(this DateTime dateTime) => dateTime.AddMinutes(60);
        public static DateTime GetRefreshTokenExpiry(this DateTime dateTime) => dateTime.AddDays(28);
        public static string GetDate(this DateOnly dateOnly) => dateOnly.ToString("O");
        public static string GetWeek(this DateOnly dateOnly) => $"{dateOnly.Year:D4}-W{calendar.GetWeekOfYear(dateOnly.ToDateTime(TimeOnly.MinValue), info.CalendarWeekRule, info.FirstDayOfWeek):D2}";
    }
}
