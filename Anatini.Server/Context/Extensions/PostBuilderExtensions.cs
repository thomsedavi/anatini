using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Extensions
{
    public static class PostBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Post> postBuilder)
        {
            postBuilder.ToContainer("Posts");
            postBuilder.HasKey(post => new { post.ChannelId, post.Id });
            postBuilder.HasPartitionKey(post => new { post.ChannelId, post.Id });
            postBuilder.Property(post => post.Id).ToJsonProperty("id");
            postBuilder.Property(post => post.ChannelId).ToJsonProperty("channelId");
            postBuilder.Property(post => post.Name).ToJsonProperty("name");
            postBuilder.Property(post => post.DateOnlyNZ).ToJsonProperty("dateOnlyNZ");
            postBuilder.Property(post => post.UpdatedDateTimeUTC).ToJsonProperty("updatedDateTimeUTC");
            postBuilder.Property(post => post.DefaultSlug).ToJsonProperty("defaultSlug");
            postBuilder.OwnsMany(post => post.Aliases, ConfigureAliases);
        }

        private static void ConfigureAliases(OwnedNavigationBuilder<Post, PostOwnedAlias> aliasesBuilder)
        {
            aliasesBuilder.ToJsonProperty("aliases");
            aliasesBuilder.WithOwner().HasForeignKey(alias => new { alias.ChannelId, alias.PostId });
            aliasesBuilder.Property(alias => alias.Slug).ToJsonProperty("slug");
            aliasesBuilder.Property(alias => alias.ChannelId).ToJsonProperty("channelId");
            aliasesBuilder.Property(alias => alias.PostId).ToJsonProperty("postId");
        }
    }
}
