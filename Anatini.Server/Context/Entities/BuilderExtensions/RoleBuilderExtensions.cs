using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class RoleBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationRole> roleBuilder)
        {
            roleBuilder.ToTable("roles");

            roleBuilder.Property(role => role.Id).Has(order: 0);
            roleBuilder.Property(role => role.Name).Has(maxLength: 256, order: 1);
            roleBuilder.Property(role => role.NormalizedName).Has(maxLength: 256, order: 2);
            roleBuilder.Property(role => role.ConcurrencyStamp).Has(order: 3).IsConcurrencyToken();

            roleBuilder.HasIndex(role => role.NormalizedName).HasDatabaseName("ix_roles_normalized_name");
        }
    }
}
