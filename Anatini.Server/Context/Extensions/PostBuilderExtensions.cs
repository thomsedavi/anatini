using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Extensions
{
    public static class PostBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Post> postBuilder)
        {
            postBuilder.ToContainer("Posts").HasPartitionKey(post => new { post.ChannelId, post.Id });
            postBuilder.OwnsMany(post => post.Aliases, alias => { alias.WithOwner().HasForeignKey(alias => new { alias.ChannelId, alias.PostId }); alias.HasKey(alias => alias.Slug); });
            postBuilder.Property(post => post.Id).ToJsonProperty("id");
            postBuilder.Property(post => post.Name).ToJsonProperty("name");
        }
    }
}
