using System.ComponentModel.DataAnnotations;

namespace Anatini.Server.Utils
{
    public class TagAttribute : ValidationAttribute
    {
        private readonly List<string> _tags = ["h1"];

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string input)
            {
                if (!_tags.Contains(input))
                {
                    return new ValidationResult("Tag must be a supported tag");
                }

                return ValidationResult.Success;
            }

            return new ValidationResult("Tag must be a string");
        }
    }
}
