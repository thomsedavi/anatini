using System.ComponentModel.DataAnnotations;
using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Works
{
    public class CreateWork
    {
        [Handle(nullable: true), MaxLength(256)]
        public string? Handle { get; set; }

        public required string Article { get; set; }

        [EnumDataType(typeof(Visibility))]
        public required Visibility Visibility { get; set; }

        [Display(Name = "Published At (NZ)")]
        public DateTime? PublishedAtNz { get; set; }

        [MaxLength(256)]
        public required string Name { get; set; }

        [Display(Name = "Is Draft")]
        public bool? IsDraft { get; set; }
    }

    public class UpdateWork
    {
        public string? Article { get; set; }
    }

    public class WorksQuery
    {
        public string? LastName { get; set; }
        public Guid? LastWorkId { get; set; }
        public int? PageSize { get; set; }
        public string? Bookmarked { get; set; }
        public string? Starred { get; set; }
        public string? Dismissed { get; set; }
        public string? Followed { get; set; }
        public string? Collected { get; set; }
    }
}
