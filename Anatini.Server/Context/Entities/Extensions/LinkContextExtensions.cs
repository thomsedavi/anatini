using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class LinkContextExtensions
    {
        public static Post AddUserLinkAsync(this ApplicationDbContext context, string name, string article, string url, Visibility visibility, Guid userId, Status status, DateTime utcNow, string? handle = null, DateTime? publishedAtNZ = null)
        {
            var linkId = Guid.CreateVersion7();

            var publishedatNz = utcNow.ConvertUtcToNz();

            if (publishedAtNZ.HasValue)
            {
                publishedatNz = publishedAtNZ.Value;
            }

            var link = new Post
            {
                Id = linkId,
                UserId = userId,
                Type = PostType.Link,
                Handle = handle ?? linkId.ToString(),
                PublishedAtNz = publishedatNz.Truncate(),
                Name = name,
                Article = article,
                Url = url,
                Visibility = visibility,
                Status = status,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            context.Add(link);

            return link;
        }

        public static Post AddSpaceLinkAsync(this ApplicationDbContext context, string name, string article, string url, Visibility visibility, Guid spaceId, Status status, DateTime utcNow, string? handle = null, DateTime? publishedAtNZ = null)
        {
            var linkId = Guid.CreateVersion7();

            var publishedatNz = utcNow.ConvertUtcToNz();

            if (publishedAtNZ.HasValue)
            {
                publishedatNz = publishedAtNZ.Value;
            }

            var link = new Post
            {
                Id = linkId,
                SpaceId = spaceId,
                Type = PostType.Link,
                Handle = handle ?? linkId.ToString(),
                PublishedAtNz = publishedatNz.Truncate(),
                Name = name,
                Article = article,
                Url = url,
                Visibility = visibility,
                Status = status,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            context.Add(link);

            return link;
        }
    }
}
