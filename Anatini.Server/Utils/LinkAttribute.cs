using System.ComponentModel.DataAnnotations;

namespace Anatini.Server.Utils
{
    public class LinkAttribute(bool nullable = false) : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // TODO
            return ValidationResult.Success;
        }
    }
}
