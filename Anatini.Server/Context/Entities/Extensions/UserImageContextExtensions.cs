using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class UserImageContextExtensions
    {
        public static async Task<int> AddUserImageAsync(this AnatiniContext context, string id, string userId, string blobContainerName, string blobName, string? altText = null)
        {
            var userImage = new UserImage
            {
                Id = id,
                ItemId = ItemId.Get(userId, id),
                UserId = userId,
                BlobContainerName = blobContainerName,
                BlobName = blobName,
                AltText = altText
            };

            return await context.AddAsync(userImage);
        }
    }
}
