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

            channelImageBuilder.Property(channelImage => channelImage.Id).Has(order: 0);
            channelImageBuilder.Property(channelImage => channelImage.ChannelId).Has(order: 1);
            channelImageBuilder.Property(channelImage => channelImage.Handle)!.Has(maxLength: 256, order: 2);
            channelImageBuilder.Property(channelImage => channelImage.BlobName)!.Has(maxLength: 64, order: 3);
            channelImageBuilder.Property(channelImage => channelImage.BlobContainerName)!.Has(maxLength: 16, order: 4);
            channelImageBuilder.Property(channelImage => channelImage.AltText).Has(maxLength: 512, order: 5);
            channelImageBuilder.Property(channelImage => channelImage.CreatedAtUtc).Has(order: 6);
            channelImageBuilder.Property(channelImage => channelImage.UpdatedAtUtc).Has(order: 7);

            channelImageBuilder.HasOneWithMany(channelImage => channelImage.Channel, channel => channel.Images, channelImage => channelImage.ChannelId, DeleteBehavior.Restrict);

            channelImageBuilder.HasIndex(channelImage => new { channelImage.ChannelId, channelImage.Handle }).IsUnique();
        }
    }
}
