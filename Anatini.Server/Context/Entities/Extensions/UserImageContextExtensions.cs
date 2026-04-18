namespace Anatini.Server.Context.Entities.Extensions
{
    public static class UserImageContextExtensions
    {
        public static ApplicationUserImage AddUserImage(this ApplicationDbContext context, Guid id, Guid userId, string handle, string blobContainerName, string blobName, string? altText = null)
        {
            var utcNow = DateTime.UtcNow;

            var userImage = new ApplicationUserImage
            {
                Id = id,
                UserId = userId,
                Handle = handle,
                BlobContainerName = blobContainerName,
                BlobName = blobName,
                AltText = altText,
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            context.Add(userImage);
            
            return userImage;
        }
    }
}
