using System.ComponentModel.DataAnnotations;

namespace Anatini.Server.Channels
{
    public class NewChannel : NewEntity
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Name { get; set; }
        [MaxLength(64), DataType(DataType.Text)]
        public required string Slug { get; set; }

        public Guid AliasGuid { get; set; } = Guid.NewGuid();
    }

    public class NewChannelAlias : NewEntity
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Slug { get; set; }
    }

    public class NewPost : NewEntity
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Name { get; set; }
        [MaxLength(64), DataType(DataType.Text)]
        public required string Slug { get; set; }

        public Guid AliasGuid { get; set; } = Guid.NewGuid();
    }
}
