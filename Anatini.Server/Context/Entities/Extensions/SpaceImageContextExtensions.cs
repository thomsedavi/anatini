namespace Anatini.Server.Context.Entities.Extensions
{
    public static class SpaceImageContextExtensions
    {
        public static SpaceImage AddSpaceImage(this ApplicationDbContext context, Guid spaceId, string handle, string blobContainerName, string blobName, string? altText = null)
        {
            var utcNow = DateTime.UtcNow;

            var spaceImage = new SpaceImage
            {
                SpaceId = spaceId,
                Handle = handle,
                BlobContainerName = blobContainerName,
                BlobName = blobName,
                AltText = altText,
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            context.Add(spaceImage);

            return spaceImage;
        }
    }
}
