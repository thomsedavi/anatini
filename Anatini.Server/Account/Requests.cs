using System.ComponentModel.DataAnnotations;

namespace Anatini.Server.Account
{
    public class UpdateUser
    {
        [MaxLength(64), DataType(DataType.Text)]
        public string? Name { get; set; }
        public Guid? IconImageId { get; set; }
    }
}
