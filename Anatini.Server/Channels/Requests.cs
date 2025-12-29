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

        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public class UpdateChannel
    {
        public Guid? DefaultCardImageId { get; set; }
    }

    public class CreateChannelAlias
    {
        [Slug, MaxLength(64), DataType(DataType.Text)]
        public required string Slug { get; set; }
    }
}
