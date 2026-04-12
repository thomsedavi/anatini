using Anatini.Server.Enums;

namespace Anatini.Server.Context.Entities
{
    public class Channel
    {
        public required Guid Id { get; set; }
        public required string Handle { get; set; }
        public required string NormalizedHandle { get; set; }
        public required string Name { get; set; }
        public required Visibility Visibility { get; set; }
        public string? About { get; set; }
        public Guid? IconImageId { get; set; }
        public Guid? BannerImageId { get; set; }
        public Guid? DefaultCardImageId { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual ICollection<ApplicationUserChannel> UserChannels { get; set; } = [];
        public virtual ICollection<Post> Posts { get; set; } = [];
        public virtual ICollection<Note> Notes { get; set; } = [];
        public virtual ICollection<Log> Logs { get; set; } = [];
        public virtual ICollection<ChannelHandle> Handles { get; set; } = [];
        public virtual ICollection<PostHandle> PostHandles { get; set; } = [];
        public virtual ICollection<ChannelImage> Images { get; set; } = [];
        public virtual ApplicationUserImage? IconImage { get; set; }
        public virtual ApplicationUserImage? BannerImage { get; set; }
        public virtual ApplicationUserImage? DefaultCardImage { get; set; }
    }

    public class ChannelHandle
    {
        public required Guid Id { get; set; }
        public required Guid ChannelId { get; set; }
        public required string Handle { get; set; }
        public required string NormalizedHandle { get; set; }
        public required DateTime CreatedAtUtc { get; set; }

        public virtual Channel Channel { get; set; } = null!;
    }

    public class ChannelImage
    {
        public required Guid Id { get; set; }
        public required Guid ChannelId { get; set; }
        public required string BlobName { get; set; }
        public required string BlobContainerName { get; set; }
        public string? AltText { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual Channel Channel { get; set; } = null!;
    }
}
