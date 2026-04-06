using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserClaimBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserClaim> userClaimBuilder)
        {
            userClaimBuilder.ToTable("user_claims");

            userClaimBuilder.HasOne(userClaim => userClaim.User).WithMany(user => user.Claims).HasForeignKey(userClaim => userClaim.UserId).IsRequired();
        }
    }
}
