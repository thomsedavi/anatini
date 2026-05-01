using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ChannelPostBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ChannelPost> channelPostBuilder)
        {
            channelPostBuilder.ToTable("channel_posts");

            channelPostBuilder.HasKey(channelPost => new { channelPost.ChannelId, channelPost.Handle });

            channelPostBuilder.Property(channelPost => channelPost.ChannelId).Has(order: 0);
            channelPostBuilder.Property(channelPost => channelPost.Handle)!.Has(maxLength: 256, order: 1);
            channelPostBuilder.Property(channelPost => channelPost.PostId).Has(order: 2);
            channelPostBuilder.Property(channelPost => channelPost.CreatedAtUtc).Has(order: 3);

            channelPostBuilder.HasOneWithMany(channelPost => channelPost.Channel, channel => channel.ChannelPosts, channelPost => channelPost.ChannelId, DeleteBehavior.Restrict);
            channelPostBuilder.HasOneWithMany(channelPost => channelPost.Post, post => post.ChannelPosts, channelPost => channelPost.PostId, DeleteBehavior.Restrict);

            channelPostBuilder.HasIndex(channelPost => new { channelPost.ChannelId, channelPost.PostId });
        }
    }
}
