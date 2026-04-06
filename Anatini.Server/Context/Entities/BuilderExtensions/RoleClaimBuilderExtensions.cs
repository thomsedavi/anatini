using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class RoleClaimBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationRoleClaim> roleClaimBuilder)
        {
            roleClaimBuilder.ToTable("role_claims");

            roleClaimBuilder.HasOne(roleClaim => roleClaim.Role).WithMany(role => role.RoleClaims).HasForeignKey(roleClaim => roleClaim.RoleId).IsRequired();
        }
    }
}
