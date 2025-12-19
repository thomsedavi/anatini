using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ChannelImageBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ChannelImage> channelImageBuilder)
        {
            channelImageBuilder.ToContainer("ChannelImages");
            channelImageBuilder.HasKey(channelImage => new { channelImage.ChannelId, channelImage.Id });
            channelImageBuilder.HasPartitionKey(channelImage => new { channelImage.ChannelId, channelImage.Id });
            channelImageBuilder.Property(channelImage => channelImage.ItemId).ToJsonProperty("id");
            channelImageBuilder.Property(channelImage => channelImage.Id).ToJsonProperty( "Id");
            channelImageBuilder.Property(channelImage => channelImage.ETag).ToJsonProperty("_etag");
            channelImageBuilder.Property(channelImage => channelImage.Timestamp).ToJsonProperty("_ts");
        }
    }
}
