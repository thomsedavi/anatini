using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Extensions
{
    public static class UserBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<User> userBuilder)
        {
            userBuilder.ToContainer("Users").HasPartitionKey(user => user.Id);
            userBuilder.OwnsMany(user => user.Sessions, session => { session.WithOwner().HasForeignKey(session => session.UserId); session.HasKey(session => session.Id); });
            userBuilder.OwnsMany(user => user.Emails, email => { email.WithOwner().HasForeignKey(email => email.UserId); email.HasKey(email => email.Address); });
            userBuilder.OwnsMany(user => user.Invites, invite => { invite.WithOwner().HasForeignKey(invite => invite.UserId); invite.HasKey(invite => invite.Code); });
            userBuilder.OwnsMany(user => user.Channels, channel => { channel.WithOwner().HasForeignKey(channel => channel.UserId); channel.HasKey(channel => channel.Id); });
            userBuilder.OwnsMany(user => user.Aliases, alias => { alias.WithOwner().HasForeignKey(alias => alias.UserId); alias.HasKey(alias => alias.Slug); });
            userBuilder.Property(user => user.Id).ToJsonProperty("id");
        }
    }
}
