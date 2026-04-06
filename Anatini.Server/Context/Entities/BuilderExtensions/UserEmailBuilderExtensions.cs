using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserEmailBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserEmail> userEmailBuilder)
        {
            userEmailBuilder.ToTable("user_emails");

            userEmailBuilder.HasKey(userEmail => userEmail.Id);

            userEmailBuilder.Property(userEmail => userEmail.Email).HasMaxLength(256);
            userEmailBuilder.Property(userEmail => userEmail.ConfirmationCode).HasMaxLength(8);
            userEmailBuilder.Property(userEmail => userEmail.NormalizedEmail).HasMaxLength(256);
            userEmailBuilder.Property(userEmail => userEmail.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            userEmailBuilder.Property(userEmail => userEmail.CreatedAtUtc).HasColumnType("timestamp with time zone");

            userEmailBuilder.HasOne(userEmail => userEmail.User).WithMany(user => user.Emails).HasForeignKey(userEmail => userEmail.UserId).OnDelete(DeleteBehavior.Cascade);

            var userIdColumnName = userEmailBuilder.GetColumnName(nameof(ApplicationUserEmail.UserId));
            userEmailBuilder.HasIndex(userEmail => userEmail.NormalizedEmail).IsUnique();
            userEmailBuilder.HasIndex(userEmail => new { userEmail.UserId, userEmail.CreatedAtUtc }).HasFilter($"{userIdColumnName} IS NULL").HasDatabaseName("ix_user_emails_null_user_id_created_at_utc");
        }
    }
}
