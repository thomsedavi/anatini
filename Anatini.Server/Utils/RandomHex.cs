using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Anatini.Server.Utils
{
    public partial class RandomHex
    {
        [GeneratedRegex(@"^[0-9a-f]{16}$")]
        private static partial Regex HexIdRegex();

        internal static string NextX8() => GetRandomHex(4);

        internal static string NextX16() => GetRandomHex(8);

        private static string GetRandomHex(int byteCount)
        {
            byte[] buffer = RandomNumberGenerator.GetBytes(byteCount);
            return Convert.ToHexString(buffer).ToLowerInvariant();
        }

        internal static bool IsX16(string id) => id != null && HexIdRegex().IsMatch(id);
    }
}
