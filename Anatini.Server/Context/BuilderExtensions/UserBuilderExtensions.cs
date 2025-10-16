using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.BuilderExtensions
{
    public static class UserBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<User> userBuilder)
        {
            userBuilder.ToContainer("Users");
            userBuilder.HasKey(user => user.Id);
            userBuilder.HasPartitionKey(user => user.Id);
            userBuilder.Property(user => user.ItemId).ToJsonProperty("id");
            userBuilder.Property(user => user.Id).ToJsonProperty("Id");
            userBuilder.OwnsMany(user => user.Aliases, buildAction => { buildAction.HasKey(userOwnedAlias => userOwnedAlias.Slug); });
            userBuilder.OwnsMany(user => user.Emails, buildAction => { buildAction.HasKey(userOwnedEmail => userOwnedEmail.Address); });
            userBuilder.OwnsMany(user => user.Invites, buildAction => { buildAction.HasKey(userOwnedInvite => userOwnedInvite.Code); });
            userBuilder.OwnsMany(user => user.Channels, buildAction => { buildAction.HasKey(userOwnedChannel => userOwnedChannel.Id); });
            userBuilder.OwnsMany(user => user.Sessions, buildAction => { buildAction.HasKey(userOwnedSession => userOwnedSession.Id); });
        }
    }
}
