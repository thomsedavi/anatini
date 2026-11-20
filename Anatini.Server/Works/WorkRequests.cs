using System.ComponentModel.DataAnnotations;
using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Works
{
    public class CreateWork
    {
        [Handle(nullable: true), MaxLength(256)]
        public string? Handle { get; set; }

        public required string Body { get; set; }

        [EnumDataType(typeof(Visibility))]
        public required Visibility Visibility { get; set; }

        [Display(Name = "Published At (NZ)")]
        public DateTime? PublishedAtNz { get; set; }

        [MaxLength(256)]
        public required string Title { get; set; }

        [EnumDataType(typeof(Status))]
        public Status? Status { get; set; }
    }

    public class UpdateWork
    {
        public string? Body { get; set; }
    }

    public class WorksQuery : ContentsQuery
    {
        public string? LastTitle { get; set; }
        public Guid? LastWorkId { get; set; }
        public int? PageSize { get; set; }
    }
}
