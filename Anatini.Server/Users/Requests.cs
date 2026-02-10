using System.ComponentModel.DataAnnotations;
using Anatini.Server.Utils;

namespace Anatini.Server.Users
{
    public class NewUserAlias
    {
        [Handle, MaxLength(64), DataType(DataType.Text)]
        public required string Handle { get; set; }
        public bool? Default { get; set; }
    }

    public class CreateRelationship
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Type { get; set; }
    }
}
