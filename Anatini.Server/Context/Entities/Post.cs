using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Context.Entities
{
    public class Post : BaseEntity
    {
        public required string ChannelId { get; set; }
        public required string Status { get; set; }
        public required ICollection<PostOwnedAlias> Aliases { get; set; }
        public required PostOwnedVersion DraftVersion { get; set; }
        public PostOwnedVersion? PublishedVersion { get; set; }
        public required string DefaultHandle { get; set; }
        public required DateTime UpdatedDateTimeUTC { get; set; }
        public bool? Protected { get; set; }
    }

    [Owned]
    public class PostOwnedVersion : PostOwnedEntity
    {
        public required string Name { get; set; }
        public string? CardImageId { get; set; }
        public string? Article { get; set; }
        public required DateOnly DateNZ { get; set; }
    }

    [Owned]
    public class PostOwnedAlias : PostOwnedEntity
    {
        public required string Handle { get; set; }
    }

    public class PostAlias : AliasEntity
    {
        public required string PostChannelId { get; set; }
        public required string PostId { get; set; }
        public required string PostName { get; set; }
        public bool? Protected { get; set; }
    }

    public class PostImage : PostEntity
    {
        public required string Id { get; set; }
    }

    public class AttributePost : PostEntity
    {
        public required string Value { get; set; }
        public required string PostHandle { get; set; }
        public required string PostChannelHandle { get; set; }
        public required string PostName { get; set; }
        public required string ChannelName { get; set; }
        public required DateOnly DateNZ { get; set; }
        public string? ChannelDefaultCardImageId { get; set; }
        public string? CardImageId { get; set; }
    }

    public abstract class PostEntity : Entity
    {
        public required string PostId { get; set; }
        public required string PostChannelId { get; set; }
    }

    public abstract class PostOwnedEntity
    {
        public required string PostChannelId { get; set; }
        public required string PostId { get; set; }
    }
}
