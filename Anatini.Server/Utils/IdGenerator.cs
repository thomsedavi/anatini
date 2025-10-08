namespace Anatini.Server.Utils
{
    public static class IdGenerator
    {
        internal static string Get(params string[] ids)
        {
            if (ids.Length == 0)
            {
                return Guid.NewGuid().ToString();
            }
            else
            {
                return string.Join('|', ids);
            }
        }
    }
}
