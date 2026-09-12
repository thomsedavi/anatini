using System.ComponentModel.DataAnnotations;
using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Posts
{
    public class CreatePost
    {
        [Handle(nullable: true), MaxLength(256)]
        public string? Handle { get; set; }

        public required string Article { get; set; }

        [EnumDataType(typeof(Visibility))]
        public required Visibility Visibility { get; set; }

        [Display(Name = "Published At (NZ)")]
        public DateTime? PublishedAtNz { get; set; }

        [MaxLength(256)]
        public string? Name { get; set; }
    }

    public class UpdatePost
    {
        public string? Article { get; set; }

        [Display(Name = "Published At (NZ)")]
        public DateTime? PublishedAtNz { get; set; }
    }

    public class PostsQuery
    {
        public DateTime? LastPublishedAtNz { get; set; }
        public Guid? LastPostId { get; set; }
        public int? PageSize { get; set; }
        public string? Bookmarked { get; set; }
        public string? Starred { get; set; }
        public string? Dismissed { get; set; }
        public string? Followed { get; set; }
        public string? Collected { get; set; }
    }
}
