using System.ComponentModel.DataAnnotations;

namespace Anatini.Server.Common
{
    public class CreateImage
    {
        public required IFormFile File { get; set; }
        [MaxLength(8), DataType(DataType.Text)]
        public required string Type { get; set; }
        [Display(Name = "Alt Text"), MaxLength(125), DataType(DataType.Text)]
        public string? AltText { get; set; }
    }
}
