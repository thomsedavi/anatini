using System.ComponentModel.DataAnnotations;

namespace Anatini.Server.Account
{
    public class UpdateUser
    {
        [MaxLength(64), DataType(DataType.Text)]
        public string? Name { get; set; }

        [MaxLength(256), DataType(DataType.Text)]
        public string? About { get; set; }

        public string? IconImageId { get; set; }

        public bool? Protected { get; set; }
    }
}
