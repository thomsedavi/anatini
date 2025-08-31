namespace Anatini.Server.Utils
{
    public static class CodeRandom
    {
        private static readonly Random random = new();

        internal static string Next()
        {
            return random.Next().ToString("X").ToLower();
        }
    }
}
