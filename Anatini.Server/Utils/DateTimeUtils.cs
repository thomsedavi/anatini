using System.Globalization;

namespace Anatini.Server.Utils
{
    public static class DateTimeUtils
    {
        private static readonly DateTimeFormatInfo Info = CultureInfo.GetCultureInfo("en-NZ").DateTimeFormat;
        private static readonly Calendar Calendar = Info.Calendar;

        public static DateTime GetAccessTokenExpiry(this DateTime dateTime) => dateTime.AddMinutes(60);
        public static DateTime GetRefreshTokenExpiry(this DateTime dateTime) => dateTime.AddDays(28);
        public static string GetDate(this DateOnly dateOnly) => dateOnly.ToString("O");
        public static string GetWeek(this DateOnly dateOnly) => $"{dateOnly.Year:D4}-W{Calendar.GetWeekOfYear(dateOnly.ToDateTime(TimeOnly.MinValue), Info.CalendarWeekRule, Info.FirstDayOfWeek):D2}";
        public static DateTime Truncate(this DateTime dateTime) => new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, dateTime.Kind);
    }
}
