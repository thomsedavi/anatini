using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Context.Entities
{
    public class Content : BaseEntity
    {
        public required string ChannelId { get; set; }
        public required string Status { get; set; }
        public required string ContentType { get; set; }
        public required ICollection<ContentOwnedAlias> Aliases { get; set; }
        public required ContentOwnedVersion DraftVersion { get; set; }
        public ContentOwnedVersion? PublishedVersion { get; set; }
        public required string DefaultHandle { get; set; }
        public required DateTime UpdatedDateTimeUTC { get; set; }
        public bool? Protected { get; set; }
    }

    [Owned]
    public class ContentOwnedVersion : ContentOwnedEntity
    {
        public required string Name { get; set; }
        public string? CardImageId { get; set; }
        public string? Article { get; set; }
        public required DateOnly DateNZ { get; set; }
    }

    [Owned]
    public class ContentOwnedAlias : ContentOwnedEntity
    {
        public required string Handle { get; set; }
    }

    public class ContentAlias : AliasEntity
    {
        public required string ContentChannelId { get; set; }
        public required string ContentId { get; set; }
        public required string ContentName { get; set; }
        public bool? Protected { get; set; }
    }

    public class ContentImage : ContentEntity
    {
        public required string Id { get; set; }
    }

    public class AttributeContent : ContentEntity
    {
        public required string Value { get; set; }
        public required string ContentHandle { get; set; }
        public required string ContentChannelHandle { get; set; }
        public required string ContentName { get; set; }
        public required string ChannelName { get; set; }
        public string? ChannelDefaultCardImageId { get; set; }
        public string? CardImageId { get; set; }
    }

    public abstract class ContentEntity : Entity
    {
        public required string ContentId { get; set; }
        public required string ContentChannelId { get; set; }
    }

    public abstract class ContentOwnedEntity
    {
        public required string ContentChannelId { get; set; }
        public required string ContentId { get; set; }
    }
}
