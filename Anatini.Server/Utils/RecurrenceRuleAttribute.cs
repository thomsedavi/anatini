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

                var dictionary = new Dictionary<string, string>();

                foreach (var component in input.Split(";"))
                {
                    var split = component.Split("=");

                    if (split.Length != 2)
                    {
                        return new ValidationResult("Incorrectly formatted Recurrence Rule");
                    }

                    if (dictionary.ContainsKey(split[0]))
                    {
                        return new ValidationResult("Duplicate key in Recurrence Rule");
                    }

                    dictionary.Add(split[0], split[1]);
                };

                if (input != "FREQ=DAILY;INTERVAL=2;COUNT=10")
                {
                    return new ValidationResult("Recurrence Rule must be this value");
                }

                return ValidationResult.Success;
            }

            return new ValidationResult("Recurrence Rule must be a string");
        }
    }
}
