using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.BuilderExtensions
{
    public static class ChannelBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Channel> channelBuilder)
        {
            channelBuilder.ToContainer("Channels");
            channelBuilder.HasKey(channel => channel.Id);
            channelBuilder.HasPartitionKey(channel => channel.Id);
            channelBuilder.Property(channel => channel.ItemId).ToJsonProperty("id");
            channelBuilder.Property(channel => channel.Id).ToJsonProperty("Id");
        }
    }
}
