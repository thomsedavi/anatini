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

            userHandleBuilder.Property(userHandle => userHandle.Handle).HasMaxLength(256);
            userHandleBuilder.Property(userHandle => userHandle.NormalizedHandle).HasMaxLength(256);
            userHandleBuilder.Property(userHandle => userHandle.CreatedAtUtc).HasColumnType("timestamp with time zone");

            userHandleBuilder.HasOne(userHandle => userHandle.User).WithMany(user => user.Handles).HasForeignKey(userHandle => userHandle.UserId).OnDelete(DeleteBehavior.Cascade);

            userHandleBuilder.HasIndex(userHandle => userHandle.NormalizedHandle).IsUnique();
        }
    }
}
