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

            userBuilder.Property(user => user.Id).Has(order: 0);
            userBuilder.Property(user => user.Handle)!.Has(maxLength: 256, order: 1);
            userBuilder.Property(user => user.Email).Has(order: 2);
            userBuilder.Property(user => user.PhoneNumber).Has(order: 3);
            userBuilder.Property(user => user.UserName).Has(order: 4);
            userBuilder.Property(user => user.Name)!.Has(maxLength: 256, order: 5);
            userBuilder.Property(user => user.Visibility).Has(order: 6);
            userBuilder.Property(user => user.About).Has(order: 7);
            userBuilder.Property(user => user.IconImageId).Has(order: 8);
            userBuilder.Property(user => user.BannerImageId).Has(order: 9);
            userBuilder.Property(user => user.PasswordHash).Has(order: 10);
            userBuilder.Property(user => user.SecurityStamp).Has(order: 11);
            userBuilder.Property(user => user.TwoFactorEnabled).Has(order: 12);
            userBuilder.Property(user => user.LockoutEnd).Has(order: 13);
            userBuilder.Property(user => user.LockoutEnabled).Has(order: 14);
            userBuilder.Property(user => user.AccessFailedCount).Has(order: 15);
            userBuilder.Property(user => user.ConcurrencyStamp).Has(order: 16).IsConcurrencyToken();
            userBuilder.Property(user => user.EmailConfirmed).Has(order: 17);
            userBuilder.Property(user => user.PhoneNumberConfirmed).Has(order: 18);
            userBuilder.Property(user => user.NormalizedHandle)!.Has(maxLength: 256, order: 19);
            userBuilder.Property(user => user.NormalizedEmail).Has(maxLength: 256, order: 20);
            userBuilder.Property(user => user.NormalizedUserName).Has(maxLength: 256, order: 21);
            userBuilder.Property(user => user.CreatedAtUtc).Has(order: 22);
            userBuilder.Property(user => user.UpdatedAtUtc).Has(order: 23);

            userBuilder.HasIndex(user => user.NormalizedHandle).IsUnique();
            userBuilder.HasIndex(user => user.NormalizedEmail).IsUnique().HasDatabaseName("ix_users_normalized_email");
            userBuilder.HasIndex(user => user.NormalizedUserName).IsUnique().HasDatabaseName("ix_users_normalized_user_name");

            userBuilder.HasOne(user => user.IconImage).WithOne().HasForeignKey<ApplicationUser>(user => user.IconImageId).OnDelete(DeleteBehavior.Restrict);
            userBuilder.HasOne(user => user.BannerImage).WithOne().HasForeignKey<ApplicationUser>(user => user.BannerImageId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
