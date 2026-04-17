using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ChannelBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Channel> channelBuilder)
        {
            channelBuilder.ToTable("channels");

            channelBuilder.HasKey(channel => channel.Id);

            channelBuilder.Property(channel => channel.Id).Has(order: 0);
            channelBuilder.Property(channel => channel.Handle)!.Has(maxLength: 256, order: 1);
            channelBuilder.Property(channel => channel.Name)!.Has(maxLength: 256, order: 2);
            channelBuilder.Property(channel => channel.Visibility).Has(order: 3);
            channelBuilder.Property(channel => channel.About).Has(maxLength: 512, order: 4);
            channelBuilder.Property(channel => channel.CreatedAtUtc).Has(order: 5);
            channelBuilder.Property(channel => channel.UpdatedAtUtc).Has(order: 6);

            channelBuilder.HasIndex(channel => channel.Handle).IsUnique();
        }
    }
}
