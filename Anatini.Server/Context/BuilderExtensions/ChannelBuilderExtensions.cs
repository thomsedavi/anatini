using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.BuilderExtensions
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
            channelBuilder.OwnsMany(channel => channel.Aliases, buildAction => { buildAction.HasKey(channelOwnedAlias => channelOwnedAlias.Slug); });
            channelBuilder.OwnsMany(channel => channel.Users, buildAction => { buildAction.HasKey(channelOwnedUser => channelOwnedUser.Id); });
            channelBuilder.OwnsMany(channel => channel.TopDraftPosts, buildAction => { buildAction.HasKey(channelOwnedPost => channelOwnedPost.Id); });
            channelBuilder.OwnsMany(channel => channel.TopPublishedPosts, buildAction => { buildAction.HasKey(channelOwnedPost => channelOwnedPost.Id); });
        }
    }
}
