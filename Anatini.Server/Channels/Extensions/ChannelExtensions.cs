using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Utils;

namespace Anatini.Server.Channels.Extensions
{
    public static class ChannelExtensions
    {
        public static Channel AddDraftContent(this Channel channel, Content content, EventData eventData)
        {
            var channelOwnedContent = new ChannelOwnedContent
            {
                Id = content.Id,
                ChannelId = channel.Id,
                Name = content.DraftVersion.Name,
                DefaultHandle = content.DefaultHandle,
                UpdatedDateTimeUTC = eventData.DateTimeUtc
            };

            var topDraftContents = channel.TopDraftContents ?? [];
            topDraftContents.Add(channelOwnedContent);
            channel.TopDraftContents = [.. topDraftContents.OrderByDescending(content => content.UpdatedDateTimeUTC).Take(8)];

            return channel;
        }

        public static Channel AddPublishedContent(this Channel channel, Content content, EventData eventData)
        {
            var channelOwnedContent = new ChannelOwnedContent
            {
                Id = content.Id,
                ChannelId = channel.Id,
                Name = content.PublishedVersion?.Name ?? "(unknown)",
                DefaultHandle = content.DefaultHandle,
                UpdatedDateTimeUTC = eventData.DateTimeUtc
            };

            // TODO top 8
            var topPublishedContents = channel.TopPublishedContents ?? [];
            topPublishedContents.Add(channelOwnedContent);
            channel.TopPublishedContents = topPublishedContents;

            return channel;
        }

        public static ChannelDto ToChannelDto(this Channel channel)
        {
            return new ChannelDto
            {
                Id = channel.Id,
                Name = channel.Name,
                TopContents = channel.TopPublishedContents?.Select(ToChannelContentDto),
                DefaultHandle = channel.DefaultHandle
            };
        }

        public static ChannelContentDto ToChannelContentDto(this ChannelOwnedContent channelOwnedContent)
        {
            return new ChannelContentDto
            {
                Name = channelOwnedContent.Name,
                DefaultHandle = channelOwnedContent.DefaultHandle
            };
        }

        public static ChannelEditDto ToChannelEditDto(this Channel channel)
        {
            return new ChannelEditDto
            {
                Id = channel.Id,
                Name = channel.Name,
                TopDraftContents = channel.TopDraftContents?.Select(ToChannelEditContentDto),
                Aliases = channel.Aliases.Select(ToChannelEditAliasDto),
                DefaultCardImageId = channel.DefaultCardImageId,
                DefaultHandle = channel.DefaultHandle,
                Protected = channel.Protected
            };
        }

        public static ChannelEditContentDto ToChannelEditContentDto(this ChannelOwnedContent channelOwnedContent)
        {
            return new ChannelEditContentDto
            {
                Id = channelOwnedContent.Id,
                Name = channelOwnedContent.Name,
                DefaultHandle = channelOwnedContent.DefaultHandle,
                UpdatedDateTimeUTC = channelOwnedContent.UpdatedDateTimeUTC
            };
        }

        public static ChannelEditAliasDto ToChannelEditAliasDto(this ChannelOwnedAlias channelOwnedAlias)
        {
            return new ChannelEditAliasDto
            {
                Handle = channelOwnedAlias.Handle
            };
        }
    }
}
