using Anatini.Server.Enums;

namespace Anatini.Server.Context.Entities
{
    public class Note
    {
        public required Guid Id { get; set; }
        public required Guid ChannelId { get; set; }
        public required PostStatus Status { get; set; }
        public required Visibility Visibility { get; set; }
        public required string Article { get; set; }
        public required string ConcurrencyStamp { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual Channel Channel { get; set; } = null!;
        public virtual ICollection<NoteImage> Images { get; set; } = [];

    }

    public class NoteImage
    {
        public Guid Id { get; set; }
        public required Guid NoteId { get; set; }
        public required string BlobName { get; set; }
        public required string BlobContainerName { get; set; }
        public string? AltText { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual Note Note { get; set; } = null!;
    }
}
