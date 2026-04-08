using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ChannelBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Channel> channelBuilder)
        {
            channelBuilder.ToTable("channels");

            channelBuilder.HasKey(channel => channel.Id);

            channelBuilder.Property(channel => channel.Id).HasColumnOrder(0);
            channelBuilder.Property(channel => channel.Handle).HasMaxLength(256).HasColumnOrder(1);
            channelBuilder.Property(channel => channel.NormalizedHandle).HasMaxLength(256).HasColumnOrder(2);
            channelBuilder.Property(channel => channel.Name).HasMaxLength(256).HasColumnOrder(3);
            channelBuilder.Property(channel => channel.Visibility).HasMaxLength(16).HasColumnOrder(4);
            channelBuilder.Property(channel => channel.About).HasMaxLength(512).HasColumnOrder(5);
            channelBuilder.Property(channel => channel.IconImageId).HasColumnOrder(6);
            channelBuilder.Property(channel => channel.BannerImageId).HasColumnOrder(7);
            channelBuilder.Property(channel => channel.DefaultCardImageId).HasColumnOrder(8);
            channelBuilder.Property(channel => channel.CreatedAtUtc).HasColumnType("timestamp with time zone").HasColumnOrder(9);
            channelBuilder.Property(channel => channel.UpdatedAtUtc).HasColumnType("timestamp with time zone").HasColumnOrder(10);

            channelBuilder.HasIndex(channel => channel.NormalizedHandle).IsUnique();
        }
    }
}
