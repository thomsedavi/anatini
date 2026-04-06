namespace Anatini.Server.Context.Entities
{
    public class Post : BaseEntity
    {
        public required string Status { get; set; }
        public required PostOwnedVersion DraftVersion { get; set; }
        public PostOwnedVersion? PublishedVersion { get; set; }

        public required Guid ChannelId { get; set; }
        public virtual Channel Channel { get; set; } = null!;
    }

    public class PostOwnedVersion
    {
        public required string Name { get; set; }
        public Guid? CardImageId { get; set; }
        public required string Article { get; set; }
        public required DateOnly DateNZ { get; set; }
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
