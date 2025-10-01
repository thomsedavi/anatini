using System.ComponentModel.DataAnnotations;

namespace Anatini.Server.Posts
{
    public class NewPost
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Name { get; set; }
        [MaxLength(64), DataType(DataType.Text)]
        public required string Slug { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
