namespace Anatini.Server.Enums
{
    [Flags]
    public enum UserNoteEdgeLabel
    {
        None = 0,
        HasDismissed = 1,
        HasStarred = 2,
        HasBookmarked = 4
    }
}
