using System.Xml.Linq;
using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class DocumentContextExtensions
    {
        public static Post AddDocument(this ApplicationDbContext context, Guid documentId, string name, string handle, Guid spaceId)
        {
            var utcNow = DateTime.UtcNow;

            var article = new XElement("article", new XElement("header", new XElement("h1", new XAttribute("tabindex", -1), name)));

            var draftVersion = new PostVersion
            {
                VersionNumber = 0,
                PostId = documentId,
                Article = article.ToString(SaveOptions.DisableFormatting),
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            var document = new Post
            {
                Id = documentId,
                SpaceId = spaceId,
                Type = PostType.Document,
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

            context.Add(document);

            return document;
        }
    }
}
