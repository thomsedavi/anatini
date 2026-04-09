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

            channelHandleBuilder.Property(channelHandle => channelHandle.Id).Has(order: 0);
            channelHandleBuilder.Property(channelHandle => channelHandle.ChannelId).Has(order: 1);
            channelHandleBuilder.Property(channelHandle => channelHandle.Handle)!.Has(maxLength: 256, order: 2);
            channelHandleBuilder.Property(channelHandle => channelHandle.NormalizedHandle)!.Has(maxLength: 256, order: 3);
            channelHandleBuilder.Property(channelHandle => channelHandle.CreatedAtUtc).Has(order: 4);

            channelHandleBuilder.HasOneWithMany(channelHandle => channelHandle.Channel, channel => channel.Handles, channelHandle => channelHandle.ChannelId, DeleteBehavior.Cascade);

            channelHandleBuilder.HasIndex(channelHandle => channelHandle.NormalizedHandle).IsUnique();
        }
    }
}
