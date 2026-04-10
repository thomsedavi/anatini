using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserChannelBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserChannel> userChannelBuilder)
        {
            userChannelBuilder.ToTable("user_channels");

            userChannelBuilder.HasKey(userChannel => new { userChannel.UserId, userChannel.ChannelId });

            userChannelBuilder.Property(userChannel => userChannel.UserId).Has(order: 0);
            userChannelBuilder.Property(userChannel => userChannel.ChannelId).Has(order: 1);
            userChannelBuilder.Property(userChannel => userChannel.CreatedAtUtc).Has(order: 2);

            userChannelBuilder.HasOneWithMany(userChannel => userChannel.User, user => user.UserChannels, userChannel => userChannel.UserId, DeleteBehavior.Restrict);
            userChannelBuilder.HasOneWithMany(userChannel => userChannel.Channel, channel => channel.UserChannels, userChannel => userChannel.ChannelId, DeleteBehavior.Restrict);
        }
    }
}
