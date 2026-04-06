using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserTokenBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserToken> userTokenBuilder)
        {
            userTokenBuilder.ToTable("user_tokens");

            userTokenBuilder.HasOne(userToken => userToken.User).WithMany(user => user.Tokens).HasForeignKey(userToken => userToken.UserId).IsRequired();
        }
    }
}
