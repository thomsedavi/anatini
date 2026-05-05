using System.Xml.Linq;
using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class PostContextExtensions
    {
        public static Post AddPost(this ApplicationDbContext context, Guid postId, string name, string handle, Guid channelId)
        {
            var utcNow = DateTime.UtcNow;

            var article = new XElement("article", new XElement("header", new XElement("h1", new XAttribute("tabindex", -1), name)));

            var draftVersion = new PostVersion
            {
                Id = Guid.CreateVersion7(),
                PostId = postId,
                Handle = "draft",
                Article = article.ToString(SaveOptions.DisableFormatting)
            };

            var post = new Post
            {
                Id = postId,
                ChannelId = channelId,
                Handle = handle,
                Status = PostStatus.Draft,
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
