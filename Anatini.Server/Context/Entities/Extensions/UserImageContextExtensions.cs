namespace Anatini.Server.Context.Entities.Extensions
{
    public static class UserImageContextExtensions
    {
        public static ApplicationUserImage AddUserImage(this ApplicationDbContext context, Guid id, Guid userId, string blobContainerName, string blobName, string? altText = null)
        {
            var userImage = new ApplicationUserImage
            {
                Id = id,
                UserId = userId,
                BlobContainerName = blobContainerName,
                BlobName = blobName,
                AltText = altText
            };

            context.Add(userImage);
            
            return userImage;
        }
    }
}
