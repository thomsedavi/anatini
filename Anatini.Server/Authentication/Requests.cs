using System.ComponentModel.DataAnnotations;
using Anatini.Server.Utils;

namespace Anatini.Server.Authentication
{
    // [JsonPropertyName] should format to camelCase but for some reason not currently working, figure out why later
    // https://learn.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-9.0#configure-formatters-2
    public class EmailForm
    {
        [Display(Name = "Email Address"), DataType(DataType.EmailAddress)]
        public required string EmailAddress { get; set; }
    }

    public class NewUser
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Name { get; set; }

        [Slug, MaxLength(64), DataType(DataType.Text)]
        public required string Slug { get; set; }

        [Display(Name = "Email Address"), DataType(DataType.EmailAddress)]
        public required string EmailAddress { get; set; }

        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Display(Name = "Verification Code"), StringLength(8), DataType(DataType.Text)]
        public required string VerificationCode { get; set; }

        public bool? Protected { get; set; }

        public string Id { get; set; } = RandomHex.NextX16();
    }

    public class LoginForm
    {
        [Display(Name = "Email Address"), DataType(DataType.EmailAddress)]
        public required string EmailAddress { get; set; }

        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }

    public class VerifyEmailForm
    {
        [Display(Name = "Email Address"), DataType(DataType.EmailAddress)]
        public required string EmailAddress { get; set; }

        [Display(Name = "Verification Code"), StringLength(8), DataType(DataType.Text)]
        public required string VerificationCode { get; set; }
    }
}
