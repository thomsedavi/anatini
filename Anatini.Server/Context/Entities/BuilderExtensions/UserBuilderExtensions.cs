using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUser> userBuilder)
        {
            userBuilder.ToTable("users");

            userBuilder.HasKey(user => user.Id);

            userBuilder.Property(user => user.Name).HasMaxLength(256);
            userBuilder.Property(user => user.Handle).HasMaxLength(256);
            userBuilder.Property(user => user.NormalizedHandle).HasMaxLength(256);
            userBuilder.Property(user => user.About).HasMaxLength(512);
            userBuilder.Property(user => user.Visibility).HasMaxLength(16);
            userBuilder.Property(user => user.CreatedAtUtc).HasColumnType("timestamp with time zone");
            userBuilder.Property(user => user.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            userBuilder.Property(user => user.ConcurrencyStamp).IsConcurrencyToken();

            userBuilder.HasIndex(user => user.NormalizedEmail).IsUnique().HasDatabaseName("ix_users_normalized_email");
            userBuilder.HasIndex(user => user.NormalizedHandle).IsUnique();
            userBuilder.HasIndex(user => user.NormalizedUserName).HasDatabaseName("ix_users_normalized_user_name");

            userBuilder.Ignore(user => user.PhoneNumber);
            userBuilder.Ignore(user => user.PhoneNumberConfirmed);
        }
    }
}
