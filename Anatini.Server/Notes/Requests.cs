using System.ComponentModel.DataAnnotations;
using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Notes
{
    public class CreateNote
    {
        [Handle(nullable: true), MaxLength(256), DataType(DataType.Text)]
        public string? Handle { get; set; }

        public required string Article { get; set; }

        [EnumDataType(typeof(Visibility))]
        public required Visibility Visibility { get; set; }

        [Display(Name = "Published At (NZ)")]
        public DateTime? PublishedAtNz { get; set; }
    }

    public class UpdateNote
    {
        public required string Article { get; set; }
    }
}
