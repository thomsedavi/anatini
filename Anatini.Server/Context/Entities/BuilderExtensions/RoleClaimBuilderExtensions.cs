using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class RoleClaimBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationRoleClaim> roleClaimBuilder)
        {
            roleClaimBuilder.ToTable("role_claims");

            roleClaimBuilder.HasKey(roleClaim => roleClaim.Id);

            roleClaimBuilder.Property(roleClaim => roleClaim.Id).Has(order: 0);
            roleClaimBuilder.Property(roleClaim => roleClaim.RoleId).Has(order: 1);
            roleClaimBuilder.Property(roleClaim => roleClaim.ClaimType).Has(order: 2);
            roleClaimBuilder.Property(roleClaim => roleClaim.ClaimValue).Has(order: 3);

            roleClaimBuilder.HasOneWithMany(roleClaim => roleClaim.Role, role => role.RoleClaims, roleClaim => roleClaim.RoleId, DeleteBehavior.Cascade);
        }
    }
}
