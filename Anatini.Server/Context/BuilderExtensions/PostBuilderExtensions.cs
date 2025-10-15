using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.BuilderExtensions
{
    public static class PostBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Post> postBuilder)
        {
            postBuilder.ToContainer("Posts");
            postBuilder.HasKey(post => new { post.ChannelId, post.Id });
            postBuilder.HasPartitionKey(post => new { post.ChannelId, post.Id });
            postBuilder.Property(post => post.ItemId).ToJsonProperty("id");
            postBuilder.OwnsMany(post => post.Aliases, buildAction => { buildAction.HasKey(postOwnedAlias => postOwnedAlias.Slug); });
        }
    }
}
