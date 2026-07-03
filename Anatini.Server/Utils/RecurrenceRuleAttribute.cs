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

                if (!dictionary.TryGetValue("FREQ", out string? frequency))
                {
                    return new ValidationResult("Recurrence Rule requires frequency");
                }
                else if (frequency == "DAILY")
                {
                    var allowedKeys = new HashSet<string> { "INTERVAL", "COUNT", "UNTIL" };

                    if (!dictionary.Keys.All(key => allowedKeys.Contains(key)))
                    {
                        return new ValidationResult("Invalid key in Recurrence Rule");
                    }

                    dictionary.TryGetValue("INTERVAL", out string? intervalString);
                    dictionary.TryGetValue("COUNT", out string? countString);
                    dictionary.TryGetValue("UNTIL", out string? untilString);

                    if (countString != null && untilString != null)
                    {
                        return new ValidationResult("Recurrence Rule must contain either 'COUNT' or 'UNTIL' but not both");
                    }
                    else if (!int.TryParse(intervalString, out int interval))
                    {
                        return new ValidationResult("Recurrence Rule interval must be integer");
                    }
                    else if (interval <= 0)
                    {
                        return new ValidationResult("Recurrence Rule interval must be greater than zero");
                    }
                    else if (!int.TryParse(countString, out int count))
                    {
                        return new ValidationResult("Recurrence Rule count must be integer");
                    }
                    else if (count <= 0)
                    {
                        return new ValidationResult("Recurrence Rule count must be greater than zero");
                    }
                    else if (!DateOnly.TryParse(untilString, out DateOnly until))
                    {
                        return new ValidationResult("Recurrence Rule until must be date only");
                    }
                    else if (until <= DateOnly.FromDateTime(DateTimeUtils.NzNow))
                    {
                        return new ValidationResult("Recurrence Rule until must be in future");
                    }

                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Recurrence Rule frequency not supported yet");
                }
            }

            return new ValidationResult("Recurrence Rule must be a string");
        }
    }
}
