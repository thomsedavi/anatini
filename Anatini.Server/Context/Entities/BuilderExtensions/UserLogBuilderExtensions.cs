using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserLogBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserLog> userLogBuilder)
        {
            userLogBuilder.ToTable("user_logs");

            userLogBuilder.HasKey(userLog => userLog.Id);

            userLogBuilder.Property(userLog => userLog.DateTimeUtc).HasColumnType("timestamp with time zone");
            userLogBuilder.Property(userLog => userLog.Type).HasMaxLength(16);

            userLogBuilder.HasOne(userLog => userLog.User).WithMany(user => user.Logs).HasForeignKey(userEvent => userEvent.UserId).OnDelete(DeleteBehavior.Cascade);
            userLogBuilder.OwnsOne(userLog => userLog.Data, builder => { builder.ToJson(); });

            userLogBuilder.HasIndex(userHandle => userHandle.DateTimeUtc);
            userLogBuilder.HasIndex(userHandle => userHandle.Type);
        }
    }
}
