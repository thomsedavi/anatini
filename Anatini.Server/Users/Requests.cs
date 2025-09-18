using System.ComponentModel.DataAnnotations;
using Anatini.Server.Authentication;

namespace Anatini.Server.Users
{
    public class NewUserSlug : NewEntity
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Slug { get; set; }
        public bool? Default { get; set; }

        public static NewUserSlug New(NewUser newUser)
        {
            var newSlug = new NewUserSlug { Slug = newUser.Slug };

            return newSlug;
        }
    }
}
