using System.Xml.Linq;
using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class PostContextExtensions
    {
        public static async Task<AttributePost> AddAttributePost(this AnatiniContext context, AttributePostType attributeType, string attributeValue, Channel channel, Post post)
        {
            var value = $"{Enum.GetName(attributeType)!}:{attributeValue}";

            var attributePost = new AttributePost
            {
                ItemId = ItemId.Get(value, channel.Id, post.Id),
                Value = value,
                PostId = post.Id,
                PostHandle = post.DefaultHandle,
                PostChannelId = channel.Id,
                PostChannelHandle = channel.DefaultHandle,
                PostName = post.DraftVersion.Name,
                ChannelName = channel.Name,
                ChannelDefaultCardImageId = channel.DefaultCardImageId,
                CardImageId = post.DraftVersion.CardImageId,
                DateNZ = post.DraftVersion.DateNZ
            };

            await context.AddAsync(attributePost);

            return attributePost;
        }

        public static async Task RemoveAttributePost(this AnatiniContext context, AttributePostType attributeType, string attributeValue, Channel channel, Post post)
        {
            var value = $"{Enum.GetName(attributeType)!}:{attributeValue}";

            var attributePost = await context.Context.AttributePosts.FindAsync(value, channel.Id, post.Id);

            if (attributePost != null)
            {
                await context.RemoveAsync(attributePost);
            }
        }

        public static async Task<Post> AddPostAsync(this AnatiniContext context, string id, string name, string handle, bool? @protected, string channelId, EventData eventData)
        {
            var article = new XElement("article", new XElement("header", new XElement("h1", new XAttribute("tabindex", -1), name)));

            var channelOwnedAlias = new PostOwnedAlias
            {
                Handle = handle,
                PostChannelId = channelId,
                PostId = id
            };

            var channelOwnedDraftVersion = new PostOwnedVersion
            {
                PostChannelId = channelId,
                Name = name,
                PostId = id,
                DateNZ = eventData.DateOnlyNZNow,
                Article = article.ToString(SaveOptions.DisableFormatting)
            };

            var post = new Post
            {
                ItemId = ItemId.Get(channelId, id),
                Id = id,
                Status = "Draft",
                ChannelId = channelId,
                Aliases = [channelOwnedAlias],
                DefaultHandle = handle,
                DraftVersion = channelOwnedDraftVersion,
                UpdatedDateTimeUTC = eventData.DateTimeUtc,
                Protected = @protected
            };

            await context.AddAsync(post);

            return post;
        }
    }
}
