using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserRoleBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserRole> userRoleBuilder)
        {
            userRoleBuilder.ToTable("user_roles");

            userRoleBuilder.HasOne(userRole => userRole.User).WithMany(user => user.Roles).HasForeignKey(userRole => userRole.UserId).IsRequired();
            userRoleBuilder.HasOne(userRole => userRole.Role).WithMany(user => user.UserRoles).HasForeignKey(userRole => userRole.RoleId).IsRequired();
        }
    }
}
