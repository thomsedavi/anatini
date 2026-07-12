using Anatini.Server.Enums;

namespace Anatini.Server.Utils
{
    public class RecurrenceRule
    {
        private readonly Frequency? frequency;
        private readonly int? interval;
        private readonly int? count;
        private readonly DateOnly? until;

        private readonly string? errorMessage;

        public RecurrenceRule(string? recurrenceRule)
        {
            if (recurrenceRule != null)
            {
                var dictionary = new Dictionary<string, string>();

                foreach (var split in recurrenceRule.Split(";"))
                {
                    var keyValue = split.Split("=");

                    if (keyValue.Length != 2)
                    {
                        errorMessage = "Incorrectly formatted Recurrence Rule";
                    }

                    if (dictionary.ContainsKey(keyValue[0]))
                    {
                        errorMessage = "Duplicate key in Recurrence Rule";
                    }

                    dictionary.Add(keyValue[0], keyValue[1]);
                }

                if (!dictionary.TryGetValue("FREQ", out string? frequencyString))
                {
                    errorMessage = "Recurrence Rule requires frequency";
                }
                else if (frequencyString == "DAILY")
                {
                    var allowedKeys = new HashSet<string> { "FREQ", "INTERVAL", "COUNT", "UNTIL" };

                    if (!dictionary.Keys.All(key => allowedKeys.Contains(key)))
                    {
                        errorMessage = "Invalid key in Recurrence Rule";
                    }

                    dictionary.TryGetValue("INTERVAL", out string? intervalString);
                    dictionary.TryGetValue("COUNT", out string? countString);
                    dictionary.TryGetValue("UNTIL", out string? untilString);

                    int? intervalValue = null;
                    int? countValue = null;
                    DateOnly? untilValue = null;

                    if (countString != null && untilString != null)
                    {
                        errorMessage = "Recurrence Rule must contain either 'COUNT' or 'UNTIL' but not both";

                        return;
                    }
                    
                    if (intervalString != null)
                    {
                        if (int.TryParse(intervalString, out int intervalResult))
                        {
                            intervalValue = intervalResult;
                        }
                        else
                        {
                            errorMessage = "Recurrence Rule interval must be integer";

                            return;
                        }
                    }
                    
                    if (intervalValue != null && intervalValue <= 0)
                    {
                        errorMessage = "Recurrence Rule interval must be greater than zero";

                        return;
                    }

                    if (countString != null)
                    {
                        if (int.TryParse(countString, out int countResult))
                        {
                            countValue = countResult;
                        }
                        else
                        {
                            errorMessage = "Recurrence Rule count must be integer";

                            return;
                        }
                    }

                    if (countValue != null && countValue <= 0)
                    {
                        errorMessage = "Recurrence Rule count must be greater than zero";

                        return;
                    }

                    if (untilString != null)
                    {
                        if (DateOnly.TryParse(untilString, out DateOnly untilResult))
                        {
                            untilValue = untilResult;
                        }
                        else
                        {
                            errorMessage = "Recurrence Rule until must be date only";

                            return;
                        }
                    }

                    // if no errors then assign values
                    frequency = Frequency.Daily;
                    interval = intervalValue;
                    count = countValue;
                    until = untilValue;
                }
                else
                {
                    errorMessage = "Recurrence Rule frequency not supported yet";
                }
            }
        }

        public string? ErrorMessage => errorMessage;

        public IEnumerable<(DateTime, DateTime)> GetInstances(DateTime startsAtNz, DateTime? endsAtNz, TimeSpan? duration)
        {
            var result = new List<(DateTime, DateTime)>();

            if (frequency == Frequency.Daily)
            {
                if (until.HasValue)
                {

                }
                else if (count.HasValue)
                {
                    for (var i = 0; i < count.Value; i++)
                    {
                        var instanceStartsAtNz = startsAtNz.AddDays(i * interval ?? 1);
                        DateTime instanceEndsAtNz;

                        if (endsAtNz.HasValue)
                        {
                            instanceEndsAtNz = instanceStartsAtNz
                                .AddDays((int)(endsAtNz.Value.Date - startsAtNz.Date).TotalDays)
                                .Date
                                .Add(TimeOnly.FromDateTime(endsAtNz.Value).ToTimeSpan());
                        }
                        else if (duration.HasValue)
                        {
                            instanceEndsAtNz = instanceStartsAtNz
                                .Add(duration.Value);
                        }
                        else
                        {
                            instanceEndsAtNz = instanceStartsAtNz
                                .AddHours(1);
                        }

                        result.Add(new (instanceStartsAtNz, instanceEndsAtNz));
                    }
                }
            }

            return result;
        }

        public DateTime? ExpiresAtNz(DateTime startsAtNz, DateTime? endsAtNz, TimeSpan? duration)
        {
            if (!count.HasValue && !until.HasValue)
            {
                return null;
            }

            if (frequency == Frequency.Daily)
            {
                DateOnly lastInstanceStartsAt;

                if (until.HasValue)
                {
                    lastInstanceStartsAt = until.Value;
                }
                else if (count.HasValue)
                {
                    lastInstanceStartsAt = DateOnly.FromDateTime(startsAtNz).AddDays(count.Value * interval ?? 1);
                }
                else
                {
                    return null;
                }

                if (endsAtNz.HasValue)
                {
                    return lastInstanceStartsAt
                        .AddDays((int)(endsAtNz.Value.Date - startsAtNz.Date).TotalDays)
                        .ToDateTime(TimeOnly.FromDateTime(endsAtNz.Value));
                }
                else if (duration.HasValue)
                {
                    return lastInstanceStartsAt
                        .ToDateTime(TimeOnly.FromDateTime(startsAtNz))
                        .Add(duration.Value);
                }
                else
                {
                    // treat midnight the following day as end of last instance
                    return lastInstanceStartsAt
                        .AddDays(1)
                        .ToDateTime(TimeOnly.MinValue);
                }
            }

            return null;
        }
    }
}
