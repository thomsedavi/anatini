using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Anatini.Server.Utils
{
    public class HandleAttribute(bool nullable = false) : ValidationAttribute
    {
        private readonly string[] _reservedWords = ["create"];

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null && nullable)
            {
                return ValidationResult.Success;
            }
            else if (value is string input)
            {
                if (Guid.TryParse(input, out Guid _))
                {
                    return new ValidationResult("Handle cannot be in this format (sorry)");
                }

                var inputLower = input.ToLower();
                var pattern = "^[a-z0-9-]+$";

                if (!Regex.IsMatch(inputLower, pattern))
                {
                    return new ValidationResult("Handle can only contain numbers, lowercase letters, and hyphens");
                }
                else if (_reservedWords.Contains(inputLower))
                {
                    return new ValidationResult($"'{input}' is a reserved word");
                }

                return ValidationResult.Success;
            }

            return new ValidationResult("Handle must be a string");
        }
    }
}
