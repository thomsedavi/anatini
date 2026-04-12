using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserClaimBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserClaim> userClaimBuilder)
        {
            userClaimBuilder.ToTable("user_claims");

            userClaimBuilder.HasKey(userLogin => userLogin.Id);

            userClaimBuilder.Property(userClaim => userClaim.Id).Has(order: 0);
            userClaimBuilder.Property(userClaim => userClaim.UserId).Has(order: 1);
            userClaimBuilder.Property(userClaim => userClaim.ClaimType).Has(order: 2);
            userClaimBuilder.Property(userClaim => userClaim.ClaimValue).Has(order: 3);

            userClaimBuilder.HasOneWithMany(userClaim => userClaim.User, user => user.Claims, userClaim => userClaim.UserId, DeleteBehavior.Cascade);
        }
    }
}
