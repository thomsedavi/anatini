using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserChannelBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserChannel> userChannelBuilder)
        {
            userChannelBuilder.ToTable("user_channels");

            userChannelBuilder.HasKey(uc => new { uc.UserId, uc.ChannelId });

            userChannelBuilder.Property(user => user.CreatedAtUtc).HasColumnType("timestamp with time zone");

            userChannelBuilder.HasOne(uc => uc.User).WithMany(u => u.UserChannels).HasForeignKey(uc => uc.UserId);
            userChannelBuilder.HasOne(uc => uc.Channel).WithMany(c => c.UserChannels).HasForeignKey(uc => uc.ChannelId);
        }
    }
}
