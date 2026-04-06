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

            channelBuilder.Property(channel => channel.Name).HasMaxLength(256);
            channelBuilder.Property(channel => channel.Handle).HasMaxLength(256);
            channelBuilder.Property(channel => channel.NormalizedHandle).HasMaxLength(256);
            channelBuilder.Property(channel => channel.About).HasMaxLength(512);
            channelBuilder.Property(channel => channel.Visibility).HasMaxLength(16);
            channelBuilder.Property(channel => channel.CreatedAtUtc).HasColumnType("timestamp with time zone");
            channelBuilder.Property(channel => channel.UpdatedAtUtc).HasColumnType("timestamp with time zone");

            channelBuilder.HasIndex(channel => channel.NormalizedHandle).IsUnique();
        }
    }
}
