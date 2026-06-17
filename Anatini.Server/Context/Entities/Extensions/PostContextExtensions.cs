using System.Xml.Linq;
using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class PostContextExtensions
    {
        public static Content AddPost(this ApplicationDbContext context, Guid postId, string name, string handle, Guid spaceId)
        {
            var utcNow = DateTime.UtcNow;

            var article = new XElement("article", new XElement("header", new XElement("h1", new XAttribute("tabindex", -1), name)));

            var draftVersion = new ContentVersion
            {
                VersionNumber = 0,
                ContentId = postId,
                Article = article.ToString(SaveOptions.DisableFormatting),
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            var post = new Content
            {
                Id = postId,
                SpaceId = spaceId,
                Type = ContentType.Post,
                CurrentVersionNumber = 0,
                Handle = handle,
                Status = Status.Draft,
                PublishedAtUtc = utcNow.Truncate(),
                Name = name,
                Visibility = Visibility.Public,
                Versions = [draftVersion],
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            context.Add(post);

            return post;
        }
    }
}
