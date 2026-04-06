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

            userTrustBuilder.Property(userTrust => userTrust.CreatedAtUtc).HasColumnType("timestamp with time zone");

            userTrustBuilder.HasOne(userTrust => userTrust.SourceUser).WithMany(user => user.GivenTrusts).HasForeignKey(userTrust => userTrust.SourceUserId).OnDelete(DeleteBehavior.Restrict);
            userTrustBuilder.HasOne(userTrust => userTrust.TargetUser).WithMany(user => user.ReceivedTrusts).HasForeignKey(userTrust => userTrust.TargetUserId).OnDelete(DeleteBehavior.Restrict);

            userTrustBuilder.HasIndex(ut => new { ut.TargetUserId, ut.SourceUserId });
        }
    }
}
