using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserTrustBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserTrust> userTrustBuilder)
        {
            userTrustBuilder.ToTable("user_trusts");

            userTrustBuilder.HasKey(userTrust => new { userTrust.SourceUserId, userTrust.TargetUserId });

            userTrustBuilder.Property(userTrust => userTrust.SourceUserId).Has(order: 0);
            userTrustBuilder.Property(userTrust => userTrust.TargetUserId).Has(order: 1);
            userTrustBuilder.Property(userTrust => userTrust.CreatedAtUtc).Has(order: 2);

            userTrustBuilder.HasOneWithMany(userTrust => userTrust.SourceUser, user => user.GivenTrusts, userTrust => userTrust.SourceUserId, DeleteBehavior.Restrict);
            userTrustBuilder.HasOneWithMany(userTrust => userTrust.TargetUser, user => user.ReceivedTrusts, userTrust => userTrust.TargetUserId, DeleteBehavior.Restrict);

            userTrustBuilder.HasIndex(ut => new { ut.TargetUserId, ut.SourceUserId });
        }
    }
}
