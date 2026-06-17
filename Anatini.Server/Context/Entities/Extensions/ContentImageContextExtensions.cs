namespace Anatini.Server.Context.Entities.Extensions
{
    public static class ContentImageContextExtensions
    {
        public static ContentImage AddContentImage(this ApplicationDbContext context, Guid contentId, string handle, string blobContainerName, string blobName, string? altText = null)
        {
            var utcNow = DateTime.UtcNow;

            var contentImage = new ContentImage
            {
                ContentId = contentId,
                Handle = handle,
                BlobContainerName = blobContainerName,
                BlobName = blobName,
                AltText = altText,
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            context.Add(contentImage);

            return contentImage;
        }
    }
}
