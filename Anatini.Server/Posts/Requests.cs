using System.ComponentModel.DataAnnotations;

namespace Anatini.Server.Posts
{
    public class NewPost : NewEntity
    {
        public required Guid ChannelId { get; set; }
        [MaxLength(64), DataType(DataType.Text)]
        public required string Name { get; set; }
        [MaxLength(64), DataType(DataType.Text)]
        public required string Slug { get; set; }

        public Guid SlugId { get; set; } = Guid.NewGuid();
    }

}
