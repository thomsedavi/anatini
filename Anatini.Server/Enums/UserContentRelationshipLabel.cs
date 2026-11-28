namespace Anatini.Server.Enums
{
    [Flags]
    public enum UserContentRelationshipLabel
    {
        None = 0,
        IsHidden = 1,
        IsLiked = 2,
        IsBookmarked = 4,
        IsCollected = 8
    }
}
