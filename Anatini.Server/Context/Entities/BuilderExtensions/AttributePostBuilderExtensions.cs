using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class AttributePostBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<AttributePost> attributePostBuilder)
        {
            attributePostBuilder.ToContainer("AttributePosts");
            attributePostBuilder.HasKey(attributePost => new { attributePost.Value, attributePost.PostChannelId, attributePost.PostId });
            attributePostBuilder.HasPartitionKey(attributePost => new { attributePost.Value, attributePost.PostChannelId, attributePost.PostId });
            attributePostBuilder.Property(attributePost => attributePost.ItemId).ToJsonProperty("id");
            attributePostBuilder.Property(attributePost => attributePost.ETag).ToJsonProperty("_etag");
            attributePostBuilder.Property(attributePost => attributePost.Timestamp).ToJsonProperty("_ts");
        }
    }
}
