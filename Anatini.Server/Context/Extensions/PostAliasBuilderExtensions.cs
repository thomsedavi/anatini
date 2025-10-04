using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Extensions
{
    public static class PostAliasBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<PostAlias> postAliasBuilder)
        {
            postAliasBuilder.ToContainer("PostAliases");
            postAliasBuilder.HasKey(postAlias => new { postAlias.ChannelId, postAlias.Slug });
            postAliasBuilder.HasPartitionKey(postAlias => new { postAlias.ChannelId, postAlias.Slug });
            postAliasBuilder.Property(postAlias => postAlias.Id).ToJsonProperty("id");
            postAliasBuilder.Property(postAlias => postAlias.Slug).ToJsonProperty("slug");
            postAliasBuilder.Property(postAlias => postAlias.ChannelId).ToJsonProperty("channelId");
            postAliasBuilder.Property(postAlias => postAlias.PostId).ToJsonProperty("postId");
            postAliasBuilder.Property(postAlias => postAlias.PostName).ToJsonProperty("postName");
        }
    }
}
