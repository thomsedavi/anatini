namespace Anatini.Server.Context.Entities.Extensions
{
    public static class ChannelImageContextExtensions
    {
        public static ChannelImage AddChannelImage(this ApplicationDbContext context, Guid id, Guid channelId, string handle, string blobContainerName, string blobName, string? altText = null)
        {
            var utcNow = DateTime.UtcNow;

            var channelImage = new ChannelImage
            {
                Id = id,
                ChannelId = channelId,
                Handle = handle,
                BlobContainerName = blobContainerName,
                BlobName = blobName,
                AltText = altText,
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            context.Add(channelImage);

            return channelImage;
        }
    }
}
