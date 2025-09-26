using System.ComponentModel.DataAnnotations;

namespace Anatini.Server.Users
{
    public class NewUserAlias : NewEntity
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Slug { get; set; }
        public bool? Default { get; set; }
    }
}
