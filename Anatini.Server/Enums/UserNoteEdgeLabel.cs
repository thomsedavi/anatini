namespace Anatini.Server.Enums
{
    [Flags]
    public enum UserNoteEdgeLabel
    {
        None = 0,
        HasSeen = 1,
        HasStarred = 2,
        HasBookmarked = 4
    }
}
