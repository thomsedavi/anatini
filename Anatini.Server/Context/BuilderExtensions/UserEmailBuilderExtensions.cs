using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.BuilderExtensions
{
    public static class UserEmailBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<UserEmail> userEmailBuilder)
        {
            userEmailBuilder.ToContainer("UserEmails");
            userEmailBuilder.HasKey(userEmail => userEmail.Address);
            userEmailBuilder.HasPartitionKey(userEmail => userEmail.Address);
            userEmailBuilder.Property(userEmail => userEmail.ItemId).ToJsonProperty("id");
            userEmailBuilder.Property(userEmail => userEmail.ETag).ToJsonProperty("_etag");
            userEmailBuilder.Property(userEmail => userEmail.Address).ToJsonProperty("Address");
        }
    }
}
