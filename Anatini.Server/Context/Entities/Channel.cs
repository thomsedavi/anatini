using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Context.Entities
{
    public class Channel : BaseEntity
    {
        public required string Name { get; set; }
        public string? About { get; set; }
        public required ICollection<ChannelOwnedUser> Users { get; set; }
        public required ICollection<ChannelOwnedAlias> Aliases { get; set; }
        public ICollection<ChannelOwnedContent>? TopDraftContents { get; set; }
        public ICollection<ChannelOwnedContent>? TopPublishedContents { get; set; }
        public required string DefaultHandle { get; set; }
        public string? IconImageId { get; set; }
        public string? BannerImageId { get; set; }
        public string? DefaultCardImageId { get; set; }
        public bool? Protected { get; set; }
    }

    [Owned]
    public class ChannelOwnedUser : ChannelOwnedEntity
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
    }

    [Owned]
    public class ChannelOwnedAlias : ChannelOwnedEntity
    {
        public required string Handle { get; set; }
    }

    [Owned]
    public class ChannelOwnedContent : ChannelOwnedEntity
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string DefaultHandle { get; set; }
        public required DateTime UpdatedDateTimeUTC { get; set; }
    }

    public class ChannelAlias : AliasEntity
    {
        public required string ChannelId { get; set; }
        public required string ChannelName { get; set; }
        public string? IconImageId { get; set; }
        public string? BannerImageId { get; set; }
        public string? DefaultCardImageId { get; set; }
        public bool? Protected { get; set; }
    }

    public class ChannelImage : ChannelEntity
    {
        public required string Id { get; set; }
        public required string BlobContainerName { get; set; }
        public required string BlobName { get; set; }
    }

    public abstract class ChannelEntity : Entity
    {
        public required string ChannelId { get; set; }
    }

    public abstract class ChannelOwnedEntity
    {
        public required string ChannelId { get; set; }
    }
}
