using System.ComponentModel.DataAnnotations;
using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Notes
{
    public class CreateNote
    {
        [Handle(nullable: true), MaxLength(256)]
        public string? Handle { get; set; }

        public required string Body { get; set; }

        [EnumDataType(typeof(Visibility))]
        public required Visibility Visibility { get; set; }

        [Display(Name = "Published At (NZ)")]
        public DateTime? PublishedAtNz { get; set; }

        [EnumDataType(typeof(Status))]
        public Status? Status { get; set; }
    }

    public class UpdateNote
    {
        public string? Body { get; set; }

        [Display(Name = "Published At (NZ)")]
        public DateTime? PublishedAtNz { get; set; }
    }

    public class NotesQuery : ContentsQuery
    {
        public DateTime? LastPublishedAtNz { get; set; }
        public Guid? LastNoteId { get; set; }
        public int? PageSize { get; set; }
    }
}
