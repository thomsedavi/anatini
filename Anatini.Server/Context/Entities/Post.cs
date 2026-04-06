using Anatini.Server.Enums;

namespace Anatini.Server.Context.Entities
{
    public class Post : BaseEntity
    {
        public required PostStatus Status { get; set; }
        public required PostOwnedVersion DraftVersion { get; set; }
        public PostOwnedVersion? PublishedVersion { get; set; }
        public string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
        public required DateOnly DateNZ { get; set; }

        public virtual ICollection<PostHandle> Handles { get; set; } = [];
        public virtual ICollection<PostImage> Images { get; set; } = [];

        public required Guid ChannelId { get; set; }
        public virtual Channel Channel { get; set; } = null!;
    }

    public class PostOwnedVersion
    {
        public required string Name { get; set; }
        public Guid? CardImageId { get; set; }
        public required string Article { get; set; }
    }

    public class PostHandle : HandleEntity
    {
        public required Guid ChannelId { get; set; }
        public virtual Channel Channel { get; set; } = null!;

        public required Guid PostId { get; set; }
        public virtual Post Post { get; set; } = null!;
    }

    public class PostImage : ImageEntity
    {
        public required Guid PostId { get; set; }
        public virtual Post Post { get; set; } = null!;
    }
}
