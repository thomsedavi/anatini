using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Extensions
{
    public static class ChannelAliasBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ChannelAlias> channelAliasBuilder)
        {
            channelAliasBuilder.ToContainer("ChannelAliases");
            channelAliasBuilder.HasKey(channelAlias => channelAlias.Slug);
            channelAliasBuilder.HasPartitionKey(channelAlias => channelAlias.Slug);
            channelAliasBuilder.Property(channelAlias => channelAlias.Slug).ToJsonProperty("id");
        }
    }
}
