using System.ComponentModel.DataAnnotations;
using Anatini.Server.Utils;

namespace Anatini.Server.Posts
{
    public class NewPost
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Name { get; set; }
        [Slug, MaxLength(64), DataType(DataType.Text)]
        public required string Slug { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
