using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserTokenBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserToken> userTokenBuilder)
        {
            userTokenBuilder.ToTable("user_tokens");

            userTokenBuilder.HasKey(userToken => new { userToken.UserId, userToken.LoginProvider, userToken.Name });

            userTokenBuilder.Property(userToken => userToken.UserId).Has(order: 0);
            userTokenBuilder.Property(userToken => userToken.LoginProvider)!.Has(order: 1);
            userTokenBuilder.Property(userToken => userToken.Name)!.Has(order: 2);
            userTokenBuilder.Property(userToken => userToken.Value).Has(order: 3);

            userTokenBuilder.HasOneWithMany(userToken => userToken.User, user => user.Tokens, userToken => userToken.UserId, DeleteBehavior.Cascade);
        }
    }
}
