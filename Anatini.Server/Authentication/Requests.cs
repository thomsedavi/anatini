using System.ComponentModel.DataAnnotations;

namespace Anatini.Server.Authentication
{
    // [JsonPropertyName] should format to camelCase but for some reason not currently working, figure out why later
    // https://learn.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-9.0#configure-formatters-2
    public class InviteCodeForm
    {
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }

        [Display(Name = "Invite Code"), StringLength(8), DataType(DataType.Text)]
        public required string InviteCode { get; set; }
    }

    public class SignupForm
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }

        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Display(Name = "Verification Code"), StringLength(8), DataType(DataType.Text)]
        public required string VerificationCode { get; set; }
    }

    public class LoginForm
    {
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }

        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }

    public class VerifyEmailForm
    {
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }

        [Display(Name = "Verification Code"), StringLength(8), DataType(DataType.Text)]
        public required string VerificationCode { get; set; }
    }
}
