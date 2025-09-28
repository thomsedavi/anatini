using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Extensions
{
    public static class PostAliasBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<PostAlias> postAliasBuilder)
        {
            postAliasBuilder.HasKey(postAlias => postAlias.Slug);
            postAliasBuilder.ToContainer("PostAliases");
            postAliasBuilder.HasPartitionKey(postAlias => new { postAlias.ChannelId, postAlias.Slug });
            postAliasBuilder.Property(postAlias => postAlias.Slug).ToJsonProperty("id");
        }
    }
}
