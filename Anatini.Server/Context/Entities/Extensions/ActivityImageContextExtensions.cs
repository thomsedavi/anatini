namespace Anatini.Server.Context.Entities.Extensions
{
    public static class ActivityImageContextExtensions
    {
        public static ActivityImage AddActivityImage(this ApplicationDbContext context, Guid activityId, string handle, string blobContainerName, string blobName, string? altText = null)
        {
            var utcNow = DateTime.UtcNow;

            var activityImage = new ActivityImage
            {
                ActivityId = activityId,
                Handle = handle,
                BlobContainerName = blobContainerName,
                BlobName = blobName,
                AltText = altText,
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            context.Add(activityImage);

            return activityImage;
        }
    }
}
