using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ChannelImageBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ChannelImage> channelImageBuilder)
        {
            channelImageBuilder.ToTable("channel_images");

            channelImageBuilder.HasKey(channelImage => channelImage.Id);

            channelImageBuilder.Property(channelImage => channelImage.AltText).HasMaxLength(256);
            channelImageBuilder.Property(channelImage => channelImage.CreatedAtUtc).HasColumnType("timestamp with time zone");
            channelImageBuilder.Property(channelImage => channelImage.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            channelImageBuilder.Property(channelImage => channelImage.BlobName).HasMaxLength(64);
            channelImageBuilder.Property(channelImage => channelImage.BlobContainerName).HasMaxLength(16);

            channelImageBuilder.HasOne(channelImage => channelImage.Channel).WithMany(user => user.Images).HasForeignKey(channelImage => channelImage.ChannelId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
