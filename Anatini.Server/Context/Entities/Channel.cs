using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Context.Entities
{
    public class Channel : BaseEntity
    {
        public required string Name { get; set; }
        public required ICollection<ChannelOwnedUser> Users { get; set; }
        public required ICollection<ChannelOwnedAlias> Aliases { get; set; }
        public ICollection<ChannelOwnedContent>? TopDraftContents { get; set; }
        public ICollection<ChannelOwnedContent>? TopPublishedContents { get; set; }
        public required string DefaultSlug { get; set; }
        public Guid? IconImageId { get; set; }
        public Guid? BannerImageId { get; set; }
        public Guid? DefaultCardImageId { get; set; }
        public bool? Protected { get; set; }
    }

    [Owned]
    public class ChannelOwnedUser : ChannelOwnedEntity
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
    }

    [Owned]
    public class ChannelOwnedAlias : ChannelOwnedEntity
    {
        public required string Slug { get; set; }
    }

    [Owned]
    public class ChannelOwnedContent : ChannelOwnedEntity
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string DefaultSlug { get; set; }
        public required DateTime UpdatedDateTimeUTC { get; set; }
    }

    public class ChannelAlias : AliasEntity
    {
        public required Guid ChannelId { get; set; }
        public required string ChannelName { get; set; }
        public Guid? IconImageId { get; set; }
        public Guid? BannerImageId { get; set; }
        public bool? Protected { get; set; }
    }

    public class ChannelImage : ChannelEntity
    {
        public required Guid Id { get; set; }
    }

    public abstract class ChannelEntity : Entity
    {
        public required Guid ChannelId { get; set; }
    }

    public abstract class ChannelOwnedEntity
    {
        public required Guid ChannelId { get; set; }
    }
}
