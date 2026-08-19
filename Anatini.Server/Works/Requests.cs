using System.ComponentModel.DataAnnotations;
using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Works
{
    public class CreateWork
    {
        [Handle(nullable: true), MaxLength(256)]
        public string? Handle { get; set; }

        public string? Article { get; set; }

        [EnumDataType(typeof(Visibility))]
        public required Visibility Visibility { get; set; }

        [Display(Name = "Published At (NZ)")]
        public DateTime? PublishedAtNz { get; set; }

        [Link]
        public required string Url { get; set; }

        [MaxLength(256)]
        public required string Name { get; set; }

        [Display(Name = "Is Draft")]
        public bool? IsDraft { get; set; }
    }

    public class UpdateWork
    {
        [Link(nullable: true)]
        public string? Url { get; set; }

        public string? Article { get; set; }
    }
}
