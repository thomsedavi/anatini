using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserLoginBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserLogin> userLoginBuilder)
        {
            userLoginBuilder.ToTable("user_logins");

            userLoginBuilder.HasOne(userLogin => userLogin.User).WithMany(user => user.Logins).HasForeignKey(userLogin => userLogin.UserId).IsRequired();
        }
    }
}
