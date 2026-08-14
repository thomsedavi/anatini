using Anatini.Server.Enums;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class WebsiteContextExtensions
    {
        public static Work AddUserWebsiteAsync(this ApplicationDbContext context, string name, string url, Visibility visibility, Guid userId, Status status, DateTime utcNow, string? handle = null, string? article = null)
        {
            var websiteId = Guid.CreateVersion7();

            var website = new Work
            {
                Id = websiteId,
                UserId = userId,
                Type = WorkType.Website,
                Handle = handle ?? websiteId.ToString(),
                Name = name,
                Article = article,
                Url = url,
                Visibility = visibility,
                Status = status,
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            context.Add(website);

            return website;
        }
    }
}
