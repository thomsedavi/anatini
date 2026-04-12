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

            userEmailBuilder.Property(userEmail => userEmail.Id).Has(order: 0);
            userEmailBuilder.Property(userEmail => userEmail.UserId).Has(order: 1);
            userEmailBuilder.Property(userEmail => userEmail.Email)!.Has(maxLength: 256, order: 2);
            userEmailBuilder.Property(userEmail => userEmail.ConfirmationCode).Has(maxLength: 8, order: 3);
            userEmailBuilder.Property(userEmail => userEmail.EmailConfirmed).Has(order: 4);
            userEmailBuilder.Property(userEmail => userEmail.NormalizedEmail)!.Has(maxLength: 256, order: 5);
            userEmailBuilder.Property(userEmail => userEmail.CreatedAtUtc).Has(order: 6);
            userEmailBuilder.Property(userEmail => userEmail.UpdatedAtUtc).Has(order: 7);

            userEmailBuilder.HasOneWithMany(userEmail => userEmail.User, user => user.Emails, userEmail => userEmail.UserId, DeleteBehavior.Cascade, required: false);

            userEmailBuilder.HasIndex(userEmail => userEmail.NormalizedEmail).IsUnique();
            userEmailBuilder.HasIndex(userEmail => new { userEmail.UserId, userEmail.CreatedAtUtc }).HasFilter($"{userEmailBuilder.GetColumnName(nameof(ApplicationUserEmail.UserId))} IS NULL").HasDatabaseName("ix_user_emails_null_user_id_created_at_utc");
        }
    }
}
