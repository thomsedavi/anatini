using Anatini.Server.Enums;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class WorkContextExtensions
    {
        public static Work AddUserWorkAsync(this ApplicationDbContext context, WorkType workType, string name, string url, Visibility visibility, Guid userId, Status status, DateTime utcNow, string? handle = null, string? article = null)
        {
            var workId = Guid.CreateVersion7();

            var work = new Work
            {
                Id = workId,
                UserId = userId,
                Type = workType,
                Handle = handle ?? workId.ToString(),
                Name = name,
                Article = article,
                Url = url,
                Visibility = visibility,
                Status = status,
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            context.Add(work);

            return work;
        }
    }
}
