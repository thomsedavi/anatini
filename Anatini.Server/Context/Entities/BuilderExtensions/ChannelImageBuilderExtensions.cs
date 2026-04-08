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

            channelImageBuilder.Property(channelImage => channelImage.Id).HasColumnOrder(0);
            channelImageBuilder.Property(channelImage => channelImage.ChannelId).HasColumnOrder(1);
            channelImageBuilder.Property(channelImage => channelImage.BlobName).HasMaxLength(64).HasColumnOrder(2);
            channelImageBuilder.Property(channelImage => channelImage.BlobContainerName).HasMaxLength(16).HasColumnOrder(3);
            channelImageBuilder.Property(channelImage => channelImage.AltText).HasMaxLength(256).HasColumnOrder(4);
            channelImageBuilder.Property(channelImage => channelImage.CreatedAtUtc).HasColumnType("timestamp with time zone").HasColumnOrder(5);
            channelImageBuilder.Property(channelImage => channelImage.UpdatedAtUtc).HasColumnType("timestamp with time zone").HasColumnOrder(6);

            channelImageBuilder.HasOne(channelImage => channelImage.Channel).WithMany(user => user.Images).HasForeignKey(channelImage => channelImage.ChannelId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
