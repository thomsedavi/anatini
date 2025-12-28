using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Context.Entities
{
    public class Content : BaseEntity
    {
        public required Guid ChannelId { get; set; }
        public required string Status { get; set; }
        public required string ContentType { get; set; }
        public required ICollection<ContentOwnedAlias> Aliases { get; set; }
        public required ContentOwnedVersion DraftVersion { get; set; }
        public ContentOwnedVersion? PublishedVersion { get; set; }
        public required string DefaultSlug { get; set; }
        public required DateTime UpdatedDateTimeUTC { get; set; }
        public bool? Protected { get; set; }
    }

    [Owned]
    public class ContentOwnedVersion : ContentOwnedEntity
    {
        public required string Name { get; set; }
        public Guid? CardImageId { get; set; }
        public ICollection<ContentOwnedElement>? Elements { get; set; }
        public required DateOnly DateNZ { get; set; }
    }

    [Owned]
    public class ContentOwnedAlias : ContentOwnedEntity
    {
        public required string Slug { get; set; }
    }

    [Owned]
    public class ContentOwnedElement
    {
        public required int Index { get; set; }
        public required string Tag { get; set; }
        public required Guid ContentOwnedVersionContentId { get; set; }
        public required Guid ContentOwnedVersionContentChannelId { get; set; }
        public string? Content { get; set; }
    }

    public class ContentAlias : AliasEntity
    {
        public required Guid ContentChannelId { get; set; }
        public required Guid ContentId { get; set; }
        public required string ContentName { get; set; }
        public bool? Protected { get; set; }
    }

    public class ContentImage : ContentEntity
    {
        public required Guid Id { get; set; }
    }

    public class AttributeContent : ContentEntity
    {
        public required string Value { get; set; }
        public required string ContentSlug { get; set; }
        public required string ContentChannelSlug { get; set; }
        public required string ContentName { get; set; }
        public required string ChannelName { get; set; }
        public Guid? ChannelDefaultCardImageId { get; set; }
        public Guid? CardImageId { get; set; }
    }

    public abstract class ContentEntity : Entity
    {
        public required Guid ContentId { get; set; }
        public required Guid ContentChannelId { get; set; }
    }

    public abstract class ContentOwnedEntity
    {
        public required Guid ContentChannelId { get; set; }
        public required Guid ContentId { get; set; }
    }
}
