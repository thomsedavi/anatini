using System.ComponentModel.DataAnnotations;

namespace Anatini.Server.Utils
{
    public class RecurrenceRuleAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            else if (value is string input)
            {
                var recurrenceRule = new RecurrenceRule(input);

                if (recurrenceRule.ErrorMessage != null)
                {
                    return new ValidationResult(recurrenceRule.ErrorMessage);
                }

                return ValidationResult.Success;
            }

            return new ValidationResult("Recurrence Rule must be a string");
        }
    }
}
