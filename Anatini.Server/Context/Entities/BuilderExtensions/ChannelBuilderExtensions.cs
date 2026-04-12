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
            channelBuilder.Property(channel => channel.NormalizedHandle)!.Has(maxLength: 256, order: 2);
            channelBuilder.Property(channel => channel.Name)!.Has(maxLength: 256, order: 3);
            channelBuilder.Property(channel => channel.Visibility).Has(order: 4);
            channelBuilder.Property(channel => channel.About).Has(maxLength: 512, order: 5);
            channelBuilder.Property(channel => channel.IconImageId).Has(order: 6);
            channelBuilder.Property(channel => channel.BannerImageId).Has(order: 7);
            channelBuilder.Property(channel => channel.DefaultCardImageId).Has(order: 8);
            channelBuilder.Property(channel => channel.CreatedAtUtc).Has(order: 9);
            channelBuilder.Property(channel => channel.UpdatedAtUtc).Has(order: 10);

            channelBuilder.HasOne(channel => channel.IconImage).WithOne().HasForeignKey<Channel>(channel => channel.IconImageId).OnDelete(DeleteBehavior.Restrict);
            channelBuilder.HasOne(channel => channel.BannerImage).WithOne().HasForeignKey<Channel>(channel => channel.BannerImageId).OnDelete(DeleteBehavior.Restrict);
            channelBuilder.HasOne(channel => channel.DefaultCardImage).WithOne().HasForeignKey<Channel>(channel => channel.DefaultCardImageId).OnDelete(DeleteBehavior.Restrict);

            channelBuilder.HasIndex(channel => channel.NormalizedHandle).IsUnique();
        }
    }
}
