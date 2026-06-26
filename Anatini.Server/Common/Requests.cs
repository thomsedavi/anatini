using System.ComponentModel.DataAnnotations;
using Anatini.Server.Utils;

namespace Anatini.Server.Common
{
    public class CreateImage
    {
        public required IFormFile File { get; set; }

        [MaxLength(8)]
        public required string Type { get; set; }

        [Handle, MaxLength(256)]
        public required string Handle { get; set; }

        [Display(Name = "Alt Text"), MaxLength(125)]
        public string? AltText { get; set; }
    }
}
