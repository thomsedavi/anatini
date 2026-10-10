namespace Anatini.Server.Enums
{
    [Flags]
    public enum UserContentRelationshipLabel
    {
        None = 0,
        IsDismissed = 1,
        IsStarred = 2,
        IsBookmarked = 4,
        IsCollected = 8
    }
}
