using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Anatini.Server.Utils
{
    public class SlugAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string input)
            {
                string pattern = "^[a-z0-9-]+$";

                if (!Regex.IsMatch(input, pattern))
                {
                    return new ValidationResult("Slug can only contain numbers, lowercase letters, and hyphens");
                }
                else if (Guid.TryParse(input, out Guid _))
                {
                    return new ValidationResult("Slug cannot be a Guid?");
                }
            }

            return ValidationResult.Success;
        }
    }
}
