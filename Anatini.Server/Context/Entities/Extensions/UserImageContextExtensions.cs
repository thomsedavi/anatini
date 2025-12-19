using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class UserImageContextExtensions
    {
        public static async Task<int> AddUserImageAsync(this AnatiniContext context, Guid id, Guid userId)
        {
            var userImage = new UserImage
            {
                Id = id,
                ItemId = ItemId.Get(userId, id),
                UserId = userId
            };

            return await context.Add(userImage);
        }
    }
}
