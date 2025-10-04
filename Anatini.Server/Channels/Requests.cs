using System.ComponentModel.DataAnnotations;
using Anatini.Server.Utils;

namespace Anatini.Server.Channels
{
    public class NewChannel
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Name { get; set; }
        [MaxLength(64), DataType(DataType.Text)]
        public required string Slug { get; set; }

        public string Id { get; set; } = IdGenerator.Get();
    }

    public class NewChannelAlias
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Slug { get; set; }
    }
}
