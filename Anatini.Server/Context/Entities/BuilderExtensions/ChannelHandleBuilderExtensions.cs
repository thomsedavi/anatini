using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ChannelHandleBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ChannelHandle> channelHandleBuilder)
        {
            channelHandleBuilder.ToTable("channel_handles");

            channelHandleBuilder.HasKey(channelHandle => channelHandle.Id);

            channelHandleBuilder.Property(channelHandle => channelHandle.Id).HasColumnOrder(0);
            channelHandleBuilder.Property(channelHandle => channelHandle.ChannelId).HasColumnOrder(1);
            channelHandleBuilder.Property(channelHandle => channelHandle.Handle).HasMaxLength(256).HasColumnOrder(2);
            channelHandleBuilder.Property(channelHandle => channelHandle.NormalizedHandle).HasMaxLength(256).HasColumnOrder(3);
            channelHandleBuilder.Property(channelHandle => channelHandle.CreatedAtUtc).HasColumnType("timestamp with time zone").HasColumnOrder(4);

            channelHandleBuilder.HasOne(channelHandle => channelHandle.Channel).WithMany(channel => channel.Handles).HasForeignKey(channelHandle => channelHandle.ChannelId).OnDelete(DeleteBehavior.Cascade);

            channelHandleBuilder.HasIndex(channelHandle => channelHandle.NormalizedHandle).IsUnique();
        }
    }
}
