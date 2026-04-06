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

            postHandleBuilder.Property(postHandle => postHandle.Handle).HasMaxLength(256);
            postHandleBuilder.Property(postHandle => postHandle.NormalizedHandle).HasMaxLength(256);
            postHandleBuilder.Property(postHandle => postHandle.CreatedAtUtc).HasColumnType("timestamp with time zone");

            postHandleBuilder.HasOne(postHandle => postHandle.Channel).WithMany(channel => channel.PostHandles).HasForeignKey(postHandle => postHandle.ChannelId).OnDelete(DeleteBehavior.Cascade);
            postHandleBuilder.HasOne(postHandle => postHandle.Post).WithMany(post => post.Handles).HasForeignKey(postHandle => postHandle.PostId).OnDelete(DeleteBehavior.Cascade);

            postHandleBuilder.HasIndex(postHandle => new { postHandle.ChannelId, postHandle.NormalizedHandle }).IsUnique();
        }
    }
}
