using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ContentAliasBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ContentAlias> contentAliasBuilder)
        {
            contentAliasBuilder.ToContainer("ContentAliases");
            contentAliasBuilder.HasKey(contentAlias => new { contentAlias.ContentChannelId, contentAlias.Handle });
            contentAliasBuilder.HasPartitionKey(contentAlias => new { contentAlias.ContentChannelId, contentAlias.Handle });
            contentAliasBuilder.Property(contentAlias => contentAlias.ItemId).ToJsonProperty("id");
            contentAliasBuilder.Property(contentAlias => contentAlias.ETag).ToJsonProperty("_etag");
            contentAliasBuilder.Property(contentAlias => contentAlias.Timestamp).ToJsonProperty("_ts");
        }
    }
}
