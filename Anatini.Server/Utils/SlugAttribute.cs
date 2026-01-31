using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Anatini.Server.Utils
{
    public class SlugAttribute : ValidationAttribute
    {
        private readonly string[] _reservedWords = ["create"];

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string input)
            {
                var inputLower = input.ToLower();
                var pattern = "^[a-z0-9-]+$";

                if (!Regex.IsMatch(inputLower, pattern))
                {
                    return new ValidationResult("Slug can only contain numbers, lowercase letters, and hyphens");
                }
                else if (Guid.TryParse(inputLower, out Guid _))
                {
                    return new ValidationResult("Slug cannot be a Guid");
                }
                else if (_reservedWords.Contains(inputLower))
                {
                    return new ValidationResult($"'{input}' is a reserved word");
                }

                return ValidationResult.Success;
            }

            return new ValidationResult("Slug must be a string");
        }
    }
}
