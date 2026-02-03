using System.ComponentModel.DataAnnotations;
using Anatini.Server.Utils;

namespace Anatini.Server.Channels
{
    public class CreateChannel
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Name { get; set; }

        [Slug, MaxLength(64), DataType(DataType.Text)]
        public required string Slug { get; set; }

        public bool? Protected { get; set; }

        public string Id { get; set; } = RandomHex.NextX16();
    }

    public class UpdateChannel
    {
        [MaxLength(64), DataType(DataType.Text)]
        public string? Name { get; set; }
        public string? DefaultCardImageId { get; set; }
    }

    public class CreateChannelAlias
    {
        [Slug, MaxLength(64), DataType(DataType.Text)]
        public required string Slug { get; set; }
    }
}
