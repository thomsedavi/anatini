using System.Xml.Linq;
using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class ContentVersionContextExtensions
    {
        // TODO this is outdated for now but might be useful later on
        public static Content AddContentVersion(this ApplicationDbContext context, Guid contentId, string? title, string? handle, Guid spaceId)
        {
            var utcNow = DateTime.UtcNow;
        
            var body = new XElement("article", new XElement("header", new XElement("h1", new XAttribute("tabindex", -1), title)));
        
            var draftVersion = new ContentVersion
            {
                VersionNumber = 0,
                ContentId = contentId,
                Body = body.ToString(SaveOptions.DisableFormatting),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };
        
            var content = new Content
            {
                Id = contentId,
                SpaceId = spaceId,
                Type = ContentType.Post,
                CurrentVersionNumber = 0,
                Handle = handle ?? contentId.ToString(),
                Status = Status.Draft,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PublishedAtNz = utcNow.ConvertUtcToNz().Truncate(),
                Title = title,
                Body = body.ToString(SaveOptions.DisableFormatting),
                Visibility = Visibility.Public,
                Versions = [draftVersion],
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };
        
            context.Add(content);
        
            return content;
        }
    }
}
