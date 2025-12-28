using SixLabors.ImageSharp;

namespace Anatini.Server.Utils
{
    public static class ImageUtils
    {
        public static async Task<(int? Width, int? Height)?> GetJpegDimensions(this IFormFile file)
        {
            using var stream = file.OpenReadStream();

            var imageInfo = await Image.IdentifyAsync(stream);

            return (imageInfo?.Width, imageInfo?.Height);
        }
    }
}
