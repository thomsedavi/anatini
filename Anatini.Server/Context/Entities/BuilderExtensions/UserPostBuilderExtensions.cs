using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserPostBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserPost> userPostBuilder)
        {
            userPostBuilder.ToTable("user_posts");

            userPostBuilder.HasKey(userPost => new { userPost.UserId, userPost.Handle });

            userPostBuilder.Property(userPost => userPost.UserId).Has(order: 0);
            userPostBuilder.Property(userPost => userPost.Handle)!.Has(maxLength: 256, order: 1);
            userPostBuilder.Property(userPost => userPost.PostId).Has(order: 2);
            userPostBuilder.Property(userPost => userPost.CreatedAtUtc).Has(order: 3);

            userPostBuilder.HasOneWithMany(userPost => userPost.User, user => user.UserPosts, userPost => userPost.UserId, DeleteBehavior.Restrict);
            userPostBuilder.HasOneWithMany(userPost => userPost.Post, post => post.UserPosts, userPost => userPost.PostId, DeleteBehavior.Restrict);
        }
    }
}
