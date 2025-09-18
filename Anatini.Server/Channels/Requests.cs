using System.ComponentModel.DataAnnotations;

namespace Anatini.Server.Channels
{
    public class NewChannel : NewEntity
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Name { get; set; }
        [MaxLength(64), DataType(DataType.Text)]
        public required string Slug { get; set; }
    }

    public class NewChannelSlug : NewEntity
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Slug { get; set; }

        public static NewChannelSlug New(NewChannel newChannel)
        {
            var newChannelSlug = new NewChannelSlug { Slug = newChannel.Slug };

            return newChannelSlug;
        }
    }
}
