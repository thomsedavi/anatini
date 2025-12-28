namespace Anatini.Server.Common
{
    public class CreateImage
    {
        public required IFormFile File { get; set; }
        public required string Type { get; set; }
    }
}
