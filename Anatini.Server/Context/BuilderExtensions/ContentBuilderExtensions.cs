using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.BuilderExtensions
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
            contentBuilder.OwnsMany(content => content.Aliases, aliasBuildAction => { aliasBuildAction.HasKey(contentOwnedAlias => contentOwnedAlias.Slug); });
            contentBuilder.OwnsOne(content => content.DraftVersion, versionBuildAction => { versionBuildAction.OwnsMany(version => version.Elements, elementBuildAction => { elementBuildAction.HasKey(element => element.Index); }); });
            contentBuilder.OwnsOne(content => content.PublishedVersion, versionbuildAction => { versionbuildAction.OwnsMany(version => version.Elements, elementBuildAction => { elementBuildAction.HasKey(element => element.Index); }); });
        }
    }
}
