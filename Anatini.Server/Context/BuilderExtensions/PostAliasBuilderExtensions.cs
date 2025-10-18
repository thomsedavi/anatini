using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.BuilderExtensions
{
    public static class PostAliasBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<PostAlias> postAliasBuilder)
        {
            postAliasBuilder.ToContainer("PostAliases");
            postAliasBuilder.HasKey(postAlias => new { postAlias.PostChannelId, postAlias.Slug });
            postAliasBuilder.HasPartitionKey(postAlias => new { postAlias.PostChannelId, postAlias.Slug });
            postAliasBuilder.Property(postAlias => postAlias.ItemId).ToJsonProperty("id");
            postAliasBuilder.Property(postAlias => postAlias.ETag).ToJsonProperty("_etag");
        }
    }
}
