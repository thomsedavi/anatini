using Anatini.Server.Enums;

namespace Anatini.Server.Context.Entities
{
    public class Project
    {
        public required Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? SpaceId { get; set; }
        public required string Handle { get; set; }
        public required ProjectType Type { get; set; }
        public DateTime? PublishedAtNz { get; set; }
        public required Status Status { get; set; }
        public required Visibility Visibility { get; set; }
        public required string Name { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public virtual Space? Space { get; set; }
        public virtual ICollection<ProjectImage> Images { get; set; } = [];
    }

    public class ProjectImage
    {
        public required Guid ProjectId { get; set; }
        public required string Handle { get; set; }
        public required string BlobName { get; set; }
        public required string BlobContainerName { get; set; }
        public string? AltText { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual Project Project { get; set; } = null!;
    }
}
