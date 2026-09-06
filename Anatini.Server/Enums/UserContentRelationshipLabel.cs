namespace Anatini.Server.Enums
{
    [Flags]
    public enum UserContentRelationshipLabel
    {
        None = 0,
        HasDismissed = 1,
        HasStarred = 2,
        HasBookmarked = 4,
        HasCollected = 8
    }
}
