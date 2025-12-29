using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class UserImageContextExtensions
    {
        public static async Task<int> AddUserImageAsync(this AnatiniContext context, Guid id, Guid userId, string blobContainerName, string blobName)
        {
            var userImage = new UserImage
            {
                Id = id,
                ItemId = ItemId.Get(userId, id),
                UserId = userId,
                BlobContainerName = blobContainerName,
                BlobName = blobName
            };

            return await context.AddAsync(userImage);
        }
    }
}
