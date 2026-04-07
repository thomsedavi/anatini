using System.ComponentModel.DataAnnotations;
using Anatini.Server.Enums;

namespace Anatini.Server.Account
{
    public class UpdateUser
    {
        [MaxLength(256), DataType(DataType.Text)]
        public string? Name { get; set; }

        [MaxLength(512), DataType(DataType.Text)]
        public string? About { get; set; }

        public Guid? IconImageId { get; set; }

        [EnumDataType(typeof(Visibility))]
        public Visibility? Visibility { get; set; }
    }
}
