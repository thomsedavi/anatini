using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class PostContextExtensions
    {
        public static Post AddUserPostAsync(this ApplicationDbContext context, string? name, string article, Visibility visibility, Guid userId, Status status, DateTime utcNow, string? handle = null, DateTime? publishedAtNZ = null)
        {
            var postId = Guid.CreateVersion7();
        
            var publishedatNz = utcNow.ConvertUtcToNz();
        
            if (publishedAtNZ.HasValue)
            {
                publishedatNz = publishedAtNZ.Value;
            }
        
            var post = new Post
            {
                Id = postId,
                UserId = userId,
                Type = ContentType.Post,
                Handle = handle ?? postId.ToString(),
                PublishedAtNz = publishedatNz.Truncate(),
                Name = name,
                Article = article,
                Visibility = visibility,
                Status = status,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };
        
            context.Add(post);
        
            return post;
        }
        
        public static Post AddSpacePostAsync(this ApplicationDbContext context, string? name, string article, Visibility visibility, Guid spaceId, Status status, DateTime utcNow, string? handle = null, DateTime? publishedAtNZ = null)
        {
            var postId = Guid.CreateVersion7();
        
            var publishedatNz = utcNow.ConvertUtcToNz();
        
            if (publishedAtNZ.HasValue)
            {
                publishedatNz = publishedAtNZ.Value;
            }
        
            var post = new Post
            {
                Id = postId,
                SpaceId = spaceId,
                Type = ContentType.Post,
                Handle = handle ?? postId.ToString(),
                PublishedAtNz = publishedatNz.Truncate(),
                Name = name,
                Article = article,
                Visibility = visibility,
                Status = status,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };
        
            context.Add(post);
        
            return post;
        }
    }
}
