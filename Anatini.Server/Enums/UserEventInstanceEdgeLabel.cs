namespace Anatini.Server.Enums
{
    [Flags]
    public enum UserEventInstanceEdgeLabel
    {
        None = 0,
        HasDismissed = 1,
        HasStarred = 2,
        HasBookmarked = 4
    }
}
