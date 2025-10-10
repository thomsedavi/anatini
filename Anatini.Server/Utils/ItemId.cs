namespace Anatini.Server.Utils
{
    public static class ItemId
    {
        internal static string Get(params object[] ids)
        {
            return string.Join('|', ids);
        }
    }
}
