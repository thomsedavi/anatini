using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserRoleBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserRole> userRoleBuilder)
        {
            userRoleBuilder.ToTable("user_roles");

            userRoleBuilder.HasKey(userRole => new { userRole.UserId, userRole.RoleId });

            userRoleBuilder.Property(userRole => userRole.UserId).Has(order: 0);
            userRoleBuilder.Property(userRole => userRole.RoleId).Has(order: 1);

            userRoleBuilder.HasOneWithMany(userRole => userRole.User, user => user.Roles, userRole => userRole.UserId, DeleteBehavior.Cascade);
            userRoleBuilder.HasOneWithMany(userRole => userRole.Role, user => user.UserRoles, userRole => userRole.RoleId, DeleteBehavior.Cascade);
        }
    }
}
