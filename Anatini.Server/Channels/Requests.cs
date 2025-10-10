using System.ComponentModel.DataAnnotations;
using Anatini.Server.Utils;

namespace Anatini.Server.Channels
{
    public class NewChannel
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Name { get; set; }
        [Slug, MaxLength(64), DataType(DataType.Text)]
        public required string Slug { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public class NewChannelAlias
    {
        [Slug, MaxLength(64), DataType(DataType.Text)]
        public required string Slug { get; set; }
    }
}
