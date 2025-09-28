using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Extensions
{
    public static class UserEmailBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<UserEmail> userEmailBuilder)
        {
            userEmailBuilder.ToContainer("UserEmails");
            userEmailBuilder.HasKey(emailAddress => emailAddress.Address);
            userEmailBuilder.HasPartitionKey(userEmail => userEmail.Address);
            userEmailBuilder.Property(userEmail => userEmail.Address).ToJsonProperty("id");
            userEmailBuilder.Property(userEmail => userEmail.UserId).ToJsonProperty("userId");
            userEmailBuilder.Property(userEmail => userEmail.InvitedByUserId).ToJsonProperty("invitedByUserId");
            userEmailBuilder.Property(userEmail => userEmail.InviteCode).ToJsonProperty("inviteCode");
            userEmailBuilder.Property(userEmail => userEmail.VerificationCode).ToJsonProperty("verificationCode");
            userEmailBuilder.Property(userEmail => userEmail.Verified).ToJsonProperty("verified");
        }
    }
}
