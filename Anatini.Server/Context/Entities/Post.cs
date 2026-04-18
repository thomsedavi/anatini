using Anatini.Server.Enums;

namespace Anatini.Server.Context.Entities
{
    public class Post
    {
        public required Guid Id { get; set; }
        public required Guid ChannelId { get; set; }
        public required string Handle { get; set; }
        public required string Name { get; set; }
        public required DateTime PublishedAtUtc { get; set; }
        public required PostStatus Status { get; set; }
        public required Visibility Visibility { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual ICollection<PostHandle> Handles { get; set; } = [];
        public virtual ICollection<PostImage> Images { get; set; } = [];
        public virtual ICollection<PostVersion> Versions { get; set; } = [];

        public virtual Channel Channel { get; set; } = null!;
    }

    public class PostVersion
    {
        public required Guid Id { get; set; }
        public required Guid PostId {  get; set; }
        public required string Handle { get; set; }
        public required string Article { get; set; }

        public virtual Post Post { get; set; } = null!;
    }

    public class PostHandle
    {
        public required Guid Id { get; set; }
        public required string Handle { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required Guid ChannelId { get; set; }
        public virtual Channel Channel { get; set; } = null!;

        public required Guid PostId { get; set; }
        public virtual Post Post { get; set; } = null!;
    }

    public class PostImage
    {
        public Guid Id { get; set; }
        public required Guid PostId { get; set; }
        public required string Handle { get; set; }
        public required string BlobName { get; set; }
        public required string BlobContainerName { get; set; }
        public string? AltText { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual Post Post { get; set; } = null!;
    }
}
