using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.BuilderExtensions
{
    public static class UserImageBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<UserImage> userImageBuilder)
        {
            userImageBuilder.ToContainer("UserImages");
            userImageBuilder.HasKey(userImage => new { userImage.UserId, userImage.Id });
            userImageBuilder.HasPartitionKey(userImage => new { userImage.UserId, userImage.Id });
            userImageBuilder.Property(userImage => userImage.ItemId).ToJsonProperty("id");
            userImageBuilder.Property(userImage => userImage.Id).ToJsonProperty( "Id");
            userImageBuilder.Property(userImage => userImage.ETag).ToJsonProperty("_etag");
            userImageBuilder.Property(userImage => userImage.Timestamp).ToJsonProperty("_ts");
        }
    }
}
