using System.ComponentModel.DataAnnotations;
using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Channels
{
    public class CreateChannel
    {
        [MaxLength(256), DataType(DataType.Text)]
        public required string Name { get; set; }

        [Handle, MaxLength(256), DataType(DataType.Text)]
        public required string Handle { get; set; }

        [EnumDataType(typeof(Visibility))]
        public required Visibility Visibility { get; set; }
    }

    public class UpdateChannel
    {
        [MaxLength(64), DataType(DataType.Text)]
        public string? Name { get; set; }
        public string? DefaultCardImageId { get; set; }
    }
}
