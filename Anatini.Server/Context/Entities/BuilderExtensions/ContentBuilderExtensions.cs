using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ContentBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Content> contentBuilder)
        {
            contentBuilder.ToContainer("Contents");
            contentBuilder.HasKey(content => new { content.ChannelId, content.Id });
            contentBuilder.HasPartitionKey(content => new { content.ChannelId, content.Id });
            contentBuilder.Property(content => content.ItemId).ToJsonProperty("id");
            contentBuilder.Property(content => content.ETag).ToJsonProperty("_etag");
            contentBuilder.Property(content => content.Timestamp).ToJsonProperty("_ts");
            contentBuilder.OwnsMany(content => content.Aliases, aliasBuildAction => { aliasBuildAction.HasKey(contentOwnedAlias => contentOwnedAlias.Slug); });
        }
    }
}
