namespace Anatini.Server.Context.Entities
{
    public class Channel : BaseEntity
    {
        public required string Name { get; set; }
        public string? About { get; set; }
        public Guid? IconImageId { get; set; }
        public Guid? BannerImageId { get; set; }
        public Guid? DefaultCardImageId { get; set; }

        public virtual ICollection<ApplicationUserChannel> UserChannels { get; set; } = [];
        public virtual ICollection<Post> Posts { get; set; } = [];
        public virtual ICollection<ChannelHandle> Handles { get; set; } = [];
        public virtual ICollection<PostHandle> PostHandles { get; set; } = [];
        public virtual ICollection<ChannelImage> Images { get; set; } = [];
    }

    public class ChannelHandle : HandleEntity
    {
        public required Guid ChannelId { get; set; }
        public virtual Channel Channel { get; set; } = null!;
    }

    public class ChannelImage : ImageEntity
    {
        public required Guid ChannelId { get; set; }
        public virtual Channel Channel { get; set; } = null!;
    }
}
