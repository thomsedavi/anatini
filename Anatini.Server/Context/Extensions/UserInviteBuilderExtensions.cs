using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Extensions
{
    public static class UserInviteBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<UserInvite> userInviteBuilder)
        {
            userInviteBuilder.ToContainer("UserInvites");
            userInviteBuilder.HasKey(userInvite => userInvite.Id);
            userInviteBuilder.HasPartitionKey(userInvite => userInvite.Id);
            userInviteBuilder.Property(userInvite => userInvite.Id).ToJsonProperty("id");
            userInviteBuilder.Property(userInvite => userInvite.Code).ToJsonProperty("code");
            userInviteBuilder.Property(userInvite => userInvite.UserId).ToJsonProperty("userId");
            userInviteBuilder.Property(userInvite => userInvite.NewUserId).ToJsonProperty("newUserId");
            userInviteBuilder.Property(userInvite => userInvite.EmailAddress).ToJsonProperty("emailAddress");
            userInviteBuilder.Property(userInvite => userInvite.DateOnlyNZ).ToJsonProperty("dateOnlyNZ");
        }
    }
}
