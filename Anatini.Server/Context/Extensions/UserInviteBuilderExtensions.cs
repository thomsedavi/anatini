using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Extensions
{
    public static class UserInviteBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<UserInvite> userInviteBuilder)
        {
            userInviteBuilder.ToContainer("UserInvites");
            userInviteBuilder.HasKey(channelAlias => channelAlias.Code);
            userInviteBuilder.HasPartitionKey(userInvite => userInvite.Code);
            userInviteBuilder.Property(userInvite => userInvite.Code).ToJsonProperty("id");
            userInviteBuilder.Property(userInvite => userInvite.InvitedByUserId).ToJsonProperty("invitedByUserId");
            userInviteBuilder.Property(userInvite => userInvite.NewUserId).ToJsonProperty("newUserId");
            userInviteBuilder.Property(userInvite => userInvite.EmailAddress).ToJsonProperty("emailAddress");
            userInviteBuilder.Property(userInvite => userInvite.DateOnlyNZ).ToJsonProperty("dateOnlyNZ");
        }
    }
}
