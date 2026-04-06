using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class RoleBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationRole> roleBuilder)
        {
            roleBuilder.ToTable("roles");

            roleBuilder.Property(role => role.ConcurrencyStamp).IsConcurrencyToken();

            roleBuilder.HasIndex(role => role.NormalizedName).HasDatabaseName("ix_roles_normalized_name");
        }
    }
}
