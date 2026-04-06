namespace Anatini.Server.Context.Entities
{
    public class Channel : Entity
    {
        public required string Name { get; set; }
        public string? Handle { get; set; }
        public string? NormalizedHandle { get; set; }
        public string? About { get; set; }
        public Guid? IconImageId { get; set; }
        public Guid? BannerImageId { get; set; }
        public Guid? DefaultCardImageId { get; set; }
        public required string Visibility { get; set; }

        public virtual ICollection<ApplicationUserChannel> UserChannels { get; set; } = [];
        public virtual ICollection<ChannelHandle> Handles { get; set; } = [];
    }

    public class ChannelHandle : Entity
    {
        public required string Handle { get; set; }
        public required string NormalizedHandle { get; set; }

        public required Guid ChannelId { get; set; }
        public virtual Channel Channel { get; set; } = null!;
    }

    public class ChannelImage : Entity
    {
        public required string BlobContainerName { get; set; }
        public required string BlobName { get; set; }
        public string? AltText { get; set; }

        public required Guid ChannelId { get; set; }
        public virtual Channel Channel { get; set; } = null!;
    }
}
