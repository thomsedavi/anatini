using System.ComponentModel.DataAnnotations;
using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Notes
{
    public class CreateNote
    {
        [Handle(nullable: true), MaxLength(256), DataType(DataType.Text)]
        public string? Handle { get; set; }

        [DataType(DataType.Text)]
        public required string Article { get; set; }

        [EnumDataType(typeof(Visibility))]
        public required Visibility Visibility { get; set; }
    }
}
