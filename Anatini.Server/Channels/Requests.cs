using System.ComponentModel.DataAnnotations;

namespace Anatini.Server.Channels
{
    public class ChannelForm
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Name { get; set; }
        [MaxLength(64), DataType(DataType.Text)]
        public required string Slug { get; set; }
    }
}
