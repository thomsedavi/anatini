using System.Security.Cryptography;

namespace Anatini.Server.Utils
{
    public static class TokenGenerator
    {
        internal static string Get
        {
            get
            {
                using var randomNumberGenerator = RandomNumberGenerator.Create();

                var bytes = new byte[32];

                randomNumberGenerator.GetBytes(bytes);

                return Convert.ToBase64String(bytes);
            }
        }
    }
}
