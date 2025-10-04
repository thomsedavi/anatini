using Microsoft.AspNetCore.WebUtilities;

namespace Anatini.Server.Utils
{
    public static class IdGenerator
    {
        internal static string Get(params string[] ids)
        {
            if (ids.Length == 0)
            {
                var guid = Guid.NewGuid();

                Span<byte> bytes = stackalloc byte[16];
                guid.TryWriteBytes(bytes);

                return WebEncoders.Base64UrlEncode(bytes);
            }
            else
            {
                return string.Join('|', ids);
            }
        }
    }
}
