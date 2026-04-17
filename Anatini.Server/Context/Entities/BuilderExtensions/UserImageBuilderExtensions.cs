using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserImageBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserImage> userImageBuilder)
        {
            userImageBuilder.ToTable("user_images");

            userImageBuilder.HasKey(userImage => userImage.Id);

            userImageBuilder.Property(userImage => userImage.Id).Has(order: 0);
            userImageBuilder.Property(userImage => userImage.UserId).Has(order: 1);
            userImageBuilder.Property(userImage => userImage.Handle)!.Has(maxLength: 256, order: 2);
            userImageBuilder.Property(userImage => userImage.BlobName)!.Has(maxLength: 64, order: 3);
            userImageBuilder.Property(userImage => userImage.BlobContainerName)!.Has(maxLength: 16, order: 4);
            userImageBuilder.Property(userImage => userImage.AltText).Has(maxLength: 512, order: 5);
            userImageBuilder.Property(userImage => userImage.CreatedAtUtc).Has(order: 6);
            userImageBuilder.Property(userImage => userImage.UpdatedAtUtc).Has(order: 7);

            userImageBuilder.HasOneWithMany(userImage => userImage.User, user => user.Images, userImage => userImage.UserId, DeleteBehavior.Restrict);

            userImageBuilder.HasIndex(userImage => new { userImage.UserId, userImage.Handle }).IsUnique();
        }
    }
}
