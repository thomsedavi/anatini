using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.BuilderExtensions
{
    public static class ContentAliasBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ContentAlias> contentAliasBuilder)
        {
            contentAliasBuilder.ToContainer("ContentAliases");
            contentAliasBuilder.HasKey(contentAlias => new { contentAlias.ContentChannelId, contentAlias.Slug });
            contentAliasBuilder.HasPartitionKey(contentAlias => new { contentAlias.ContentChannelId, contentAlias.Slug });
            contentAliasBuilder.Property(contentAlias => contentAlias.ItemId).ToJsonProperty("id");
            contentAliasBuilder.Property(contentAlias => contentAlias.ETag).ToJsonProperty("_etag");
        }
    }
}
