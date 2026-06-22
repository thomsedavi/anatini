using System.ComponentModel.DataAnnotations;

namespace Anatini.Server.Events
{
    public class CreateEvent
    {
        [MaxLength(256), DataType(DataType.Text)]
        public required string Name { get; set; }

        [Display(Name = "Starts At (NZ)")]
        public required DateTime StartsAtNz { get; set; }
    }
}
