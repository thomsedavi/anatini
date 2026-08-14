using System.ComponentModel.DataAnnotations;
using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Posts.Links
{
    public class CreateLink
    {
        [Handle(nullable: true), MaxLength(256)]
        public string? Handle { get; set; }

        [MaxLength(256)]
        public required string Name { get; set; }

        public required string Article { get; set; }

        [Link]
        public required string Url { get; set; }

        [EnumDataType(typeof(Visibility))]
        public required Visibility Visibility { get; set; }

        [Display(Name = "Published At (NZ)")]
        public DateTime? PublishedAtNz { get; set; }
    }

    public class UpdateLink
    {
        [MaxLength(256)]
        public string? Name { get; set; }

        public string? Article { get; set; }

        [Link]
        public string? Url { get; set; }

        [Display(Name = "Published At (NZ)")]
        public DateTime? PublishedAtNz { get; set; }
    }
}
