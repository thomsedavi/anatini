using Anatini.Server.Enums;

namespace Anatini.Server.Context.Entities
{
    public class Space
    {
        public required Guid Id { get; set; }
        public required string Handle { get; set; }
        public required string Name { get; set; }
        public required Visibility Visibility { get; set; }
        public string? About { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual ICollection<ApplicationUserSpaceEdge> UserEdges { get; set; } = [];
        public virtual ICollection<Log> Logs { get; set; } = [];
        public virtual ICollection<SpaceHandle> Handles { get; set; } = [];
        public virtual ICollection<SpaceImage> Images { get; set; } = [];
        public virtual ICollection<Content> Contents { get; set; } = [];
        public virtual ICollection<EventSeries> EventSeries { get; set; } = [];
        public virtual ICollection<EventInstance> EventInstances { get; set; } = [];
    }

    public class SpaceHandle
    {
        public required Guid Id { get; set; }
        public required Guid SpaceId { get; set; }
        public required string Handle { get; set; }
        public required DateTime CreatedAtUtc { get; set; }

        public virtual Space Space { get; set; } = null!;
    }

    public class SpaceImage
    {
        public required Guid SpaceId { get; set; }
        public required string Handle { get; set; }
        public required string BlobName { get; set; }
        public required string BlobContainerName { get; set; }
        public string? AltText { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual Space Space { get; set; } = null!;
    }
}
