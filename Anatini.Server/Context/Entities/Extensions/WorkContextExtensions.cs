using Anatini.Server.Enums;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class WorkContextExtensions
    {
        public static Work AddUserWorkAsync(this ApplicationDbContext context, string name, Visibility visibility, Guid userId, Status status, DateTime utcNow, string? handle = null, string? article = null, string? url = null)
        {
            var workId = Guid.CreateVersion7();

            var work = new Work
            {
                Id = workId,
                UserId = userId,
                Type = ContentType.Work,
                Handle = handle ?? workId.ToString(),
                Name = name,
                Article = article,
                Url = url,
                Visibility = visibility,
                Status = status,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            context.Add(work);

            return work;
        }

        public static Work AddSpaceWorkAsync(this ApplicationDbContext context, string name, Visibility visibility, Guid spaceId, Status status, DateTime utcNow, string? handle = null, string? article = null, string? url = null)
        {
            var workId = Guid.CreateVersion7();

            var work = new Work
            {
                Id = workId,
                SpaceId = spaceId,
                Type = ContentType.Work,
                Handle = handle ?? workId.ToString(),
                Name = name,
                Article = article,
                Url = url,
                Visibility = visibility,
                Status = status,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            context.Add(work);

            return work;
        }
    }
}
