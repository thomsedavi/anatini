using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class PostBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Post> postBuilder)
        {
            postBuilder.ToContainer("Posts");
            postBuilder.HasKey(post => new { post.ChannelId, post.Id });
            postBuilder.HasPartitionKey(post => new { post.ChannelId, post.Id });
            postBuilder.Property(post => post.ItemId).ToJsonProperty("id");
            postBuilder.Property(post => post.ETag).ToJsonProperty("_etag");
            postBuilder.Property(post => post.Timestamp).ToJsonProperty("_ts");
            postBuilder.OwnsMany(post => post.Aliases, aliasBuildAction => { aliasBuildAction.HasKey(postOwnedAlias => postOwnedAlias.Handle); });
        }
    }
}
