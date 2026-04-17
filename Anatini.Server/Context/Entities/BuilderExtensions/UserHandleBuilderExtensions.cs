using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserHandleBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserHandle> userHandleBuilder)
        {
            userHandleBuilder.ToTable("user_handles");

            userHandleBuilder.HasKey(userHandle => userHandle.Id);

            userHandleBuilder.Property(userHandle => userHandle.Id).Has(order: 0);
            userHandleBuilder.Property(userHandle => userHandle.UserId).Has(order: 1);
            userHandleBuilder.Property(userHandle => userHandle.Handle)!.Has(maxLength:256, order: 2);
            userHandleBuilder.Property(userHandle => userHandle.CreatedAtUtc).Has(order: 3);

            userHandleBuilder.HasOneWithMany(userHandle => userHandle.User, user => user.Handles, userHandle => userHandle.UserId, DeleteBehavior.Cascade);

            userHandleBuilder.HasIndex(userHandle => userHandle.Handle).IsUnique();
        }
    }
}
