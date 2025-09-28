using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Extensions
{
    public static class ChannelBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Channel> channelBuilder)
        {
            channelBuilder.ToContainer("Channels").HasPartitionKey(channel => channel.Id);
            channelBuilder.OwnsMany(channel => channel.Users, user => { user.WithOwner().HasForeignKey(user => user.ChannelId); user.HasKey(user => user.Id); });
            channelBuilder.OwnsMany(channel => channel.Aliases, alias => { alias.WithOwner().HasForeignKey(alias => alias.ChannelId); alias.HasKey(alias => alias.Slug); });
            channelBuilder.OwnsMany(channel => channel.TopDraftPosts, post => { post.WithOwner().HasForeignKey(post => post.ChannelId); post.HasKey(post => post.Id); });
            channelBuilder.OwnsMany(channel => channel.TopPublishedPosts, post => { post.WithOwner().HasForeignKey(post => post.ChannelId); post.HasKey(post => post.Id); });
            channelBuilder.Property(channel => channel.Id).ToJsonProperty("id");
        }
    }
}
