using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ChannelBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Channel> channelBuilder)
        {
            channelBuilder.ToContainer("Channels");
            channelBuilder.HasKey(channel => channel.Id);
            channelBuilder.HasPartitionKey(channel => channel.Id);
            channelBuilder.Property(channel => channel.ItemId).ToJsonProperty("id");
            channelBuilder.Property(channel => channel.Id).ToJsonProperty("Id");
            channelBuilder.Property(channel => channel.ETag).ToJsonProperty("_etag");
            channelBuilder.Property(channel => channel.Timestamp).ToJsonProperty("_ts");
            channelBuilder.OwnsMany(channel => channel.Aliases, aliasBuildAction => { aliasBuildAction.HasKey(channelOwnedAlias => channelOwnedAlias.Slug); });
            channelBuilder.OwnsMany(channel => channel.Users, userBuildAction => { userBuildAction.HasKey(channelOwnedUser => channelOwnedUser.Id); });
            channelBuilder.OwnsMany(channel => channel.TopDraftContents, contentBuildAction => { contentBuildAction.HasKey(channelOwnedContent => channelOwnedContent.Id); });
            channelBuilder.OwnsMany(channel => channel.TopPublishedContents, contentBuildAction => { contentBuildAction.HasKey(channelOwnedContent => channelOwnedContent.Id); });
        }
    }
}
