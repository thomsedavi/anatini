using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserLoginBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserLogin> userLoginBuilder)
        {
            userLoginBuilder.ToTable("user_logins");

            userLoginBuilder.HasKey(userLogin => new { userLogin.LoginProvider, userLogin.ProviderKey });

            userLoginBuilder.Property(userLogin => userLogin.LoginProvider)!.Has(order: 0);
            userLoginBuilder.Property(userLogin => userLogin.ProviderKey)!.Has(order: 1);
            userLoginBuilder.Property(userLogin => userLogin.ProviderDisplayName).Has(order: 2);
            userLoginBuilder.Property(userLogin => userLogin.UserId).Has(order: 3);

            userLoginBuilder.HasOneWithMany(userLogin => userLogin.User, user => user.Logins, userLogin => userLogin.UserId, DeleteBehavior.Cascade);
        }
    }
}
