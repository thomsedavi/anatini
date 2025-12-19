using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ContentImageBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ContentImage> contentImageBuilder)
        {
            contentImageBuilder.ToContainer("ContentImages");
            contentImageBuilder.HasKey(contentImage => new { contentImage.ContentChannelId, contentImage.ContentId, contentImage.Id });
            contentImageBuilder.HasPartitionKey(contentImage => new { contentImage.ContentChannelId, contentImage.ContentId, contentImage.Id });
            contentImageBuilder.Property(contentImage => contentImage.ItemId).ToJsonProperty("id");
            contentImageBuilder.Property(contentImage => contentImage.Id).ToJsonProperty( "Id");
            contentImageBuilder.Property(contentImage => contentImage.ETag).ToJsonProperty("_etag");
            contentImageBuilder.Property(contentImage => contentImage.Timestamp).ToJsonProperty("_ts");
        }
    }
}
