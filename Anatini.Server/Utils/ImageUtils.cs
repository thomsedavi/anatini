using SixLabors.ImageSharp;

namespace Anatini.Server.Utils
{
    public static class ImageUtils
    {
        public static (int? Width, int? Height)? GetJpegDimensions(this IFormFile file)
        {
            using var stream = file.OpenReadStream();

            var imageInfo = Image.Identify(stream);

            return (imageInfo?.Width, imageInfo?.Height);
        }
    }
}
