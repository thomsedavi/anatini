using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.BuilderExtensions
{
    public static class UserBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<User> userBuilder)
        {
            userBuilder.ToContainer("Users");
            userBuilder.HasKey(user => user.Id);
            userBuilder.HasPartitionKey(user => user.Id);
            userBuilder.Property(user => user.ItemId).ToJsonProperty("id");
            userBuilder.Property(user => user.Id).ToJsonProperty("Id");
        }
    }
}
