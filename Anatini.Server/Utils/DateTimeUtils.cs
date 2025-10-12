namespace Anatini.Server.Utils
{
    public static class DateTimeUtils
    {
        public static DateTime GetAccessTokenExpiry(this DateTime utcNow) => utcNow.AddMinutes(60);
        public static DateTime GetRefreshTokenExpiry(this DateTime utcNow) => utcNow.AddDays(28);
    }
}
