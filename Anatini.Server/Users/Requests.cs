using System.ComponentModel.DataAnnotations;

namespace Anatini.Server.Users
{
    public class HandleForm
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Handle { get; set; }
        public bool? Default { get; set; }
    }
}
