using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.BuilderExtensions
{
    public static class AttributeContentBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<AttributeContent> attributeContentBuilder)
        {
            attributeContentBuilder.ToContainer("AttributeContents");
            attributeContentBuilder.HasKey(attributeContent => new { attributeContent.Value, attributeContent.ContentChannelId, attributeContent.ContentId });
            attributeContentBuilder.HasPartitionKey(attributeContent => new { attributeContent.Value, attributeContent.ContentChannelId, attributeContent.ContentId });
            attributeContentBuilder.Property(attributeContent => attributeContent.ItemId).ToJsonProperty("id");
            attributeContentBuilder.Property(attributeContent => attributeContent.ETag).ToJsonProperty("_etag");
            attributeContentBuilder.Property(attributeContent => attributeContent.Timestamp).ToJsonProperty("_ts");
        }
    }
}
