using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class PostHandleBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<PostHandle> postHandleBuilder)
        {
            postHandleBuilder.ToTable("post_handles");

            postHandleBuilder.HasKey(postHandle => postHandle.Id);

            postHandleBuilder.Property(postHandle => postHandle.Id).Has(order: 0);
            postHandleBuilder.Property(postHandle => postHandle.PostId).Has(order: 1);
            postHandleBuilder.Property(postHandle => postHandle.ChannelId).Has(order: 2);
            postHandleBuilder.Property(postHandle => postHandle.Handle)!.Has(maxLength: 256, order: 4);
            postHandleBuilder.Property(postHandle => postHandle.NormalizedHandle)!.Has(maxLength: 256, order: 5);
            postHandleBuilder.Property(postHandle => postHandle.CreatedAtUtc).Has(order: 6);

            postHandleBuilder.HasOneWithMany(postHandle => postHandle.Channel, channel => channel.PostHandles, postHandle => postHandle.ChannelId, DeleteBehavior.Restrict);
            postHandleBuilder.HasOneWithMany(postHandle => postHandle.Post, post => post.Handles, postHandle => postHandle.PostId, DeleteBehavior.Restrict);

            postHandleBuilder.HasIndex(postHandle => new { postHandle.ChannelId, postHandle.NormalizedHandle }).IsUnique();
        }
    }
}
