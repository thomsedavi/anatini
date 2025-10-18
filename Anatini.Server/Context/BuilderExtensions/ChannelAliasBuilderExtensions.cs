using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.BuilderExtensions
{
    public static class ChannelAliasBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ChannelAlias> channelAliasBuilder)
        {
            channelAliasBuilder.ToContainer("ChannelAliases");
            channelAliasBuilder.HasKey(channelAlias => channelAlias.Slug);
            channelAliasBuilder.HasPartitionKey(channelAlias => channelAlias.Slug);
            channelAliasBuilder.Property(channelAlias => channelAlias.ItemId).ToJsonProperty("id");
            channelAliasBuilder.Property(channelAlias => channelAlias.ETag).ToJsonProperty("_etag");
            channelAliasBuilder.Property(channel => channel.Slug).ToJsonProperty("Slug");
        }
    }
}
