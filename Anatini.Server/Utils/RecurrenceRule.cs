using Anatini.Server.Enums;

namespace Anatini.Server.Utils
{
    public class RecurrenceRule
    {
        private readonly Frequency? frequency;
        private readonly int? interval;
        private readonly int? count;
        private readonly DateOnly? until;
        private readonly int? byDayIndex;
        private readonly string? byDayDay;
        private readonly int? byMonthDay;
        private readonly string[]? byDayDays;

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

                    frequency = Frequency.Daily;
                }
                else if (frequencyString == "WEEKLY")
                {
                    var allowedKeys = new HashSet<string> { "FREQ", "INTERVAL", "COUNT", "UNTIL", "BYDAY" };

                    if (!dictionary.Keys.All(key => allowedKeys.Contains(key)))
                    {
                        errorMessage = "Invalid key in Recurrence Rule";
                    }

                    dictionary.TryGetValue("BYDAY", out string? byDayString);

                    string[]? byDayDaysValue = null;

                    if (byDayString != null)
                    {
                        byDayDaysValue = byDayString.Split(",");

                        var allowedByDayDays = new HashSet<string> { "MO", "TU", "WE", "TH", "FR", "SA", "SU" };

                        if (byDayDaysValue.Any(byDayDay => !allowedByDayDays.Contains(byDayDay)))
                        {
                            errorMessage = "Recurrence Rule must have a by day";

                            return;
                        }
                    }
                    else
                    {
                        errorMessage = "Recurrence Rule must have a by day";

                        return;
                    }

                    frequency = Frequency.Weekly;
                    byDayDays = byDayDaysValue;
                }
                else if (frequencyString == "MONTHLY")
                {
                    var allowedKeys = new HashSet<string> { "FREQ", "INTERVAL", "COUNT", "UNTIL", "BYDAY", "BYMONTHDAY" };

                    if (!dictionary.Keys.All(key => allowedKeys.Contains(key)))
                    {
                        errorMessage = "Invalid key in Recurrence Rule";
                    }

                    dictionary.TryGetValue("BYMONTHDAY", out string? byMonthDayString);
                    dictionary.TryGetValue("BYDAY", out string? byDayString);

                    int? byDayIndexValue = null;
                    int? byMonthDayValue = null;
                    string? byDayDayValue = null;

                    if (byMonthDayString != null)
                    {
                        if (int.TryParse(byMonthDayString, out int byMonthDayResult))
                        {
                            byMonthDayValue = byMonthDayResult;
                        }
                        else
                        {
                            errorMessage = "Recurrence Rule by month day must be integer";

                            return;
                        }
                    }

                    if (byMonthDayValue != null && (byMonthDayValue < 1 || byMonthDayValue > 31))
                    {
                        errorMessage = "Recurrence Rule by month day must be between 1 and 31";

                        return;
                    }

                    if (byDayString != null)
                    {
                        if (byDayString.Length < 3)
                        {
                            errorMessage = "By Day has a minimum length of three characters";

                            return;
                        }

                        var byDayDay = byDayString[^2..];

                        var allowedByDayDays = new HashSet<string> { "MO", "TU", "WE", "TH", "FR", "SA", "SU" };

                        if (!allowedByDayDays.Contains(byDayDay))
                        {
                            errorMessage = "By Day invalid";

                            return;
                        }

                        byDayDayValue = byDayDay;

                        var byDayIndex = byDayString[..^2];

                        if (int.TryParse(byDayIndex, out int byDayIndexResult))
                        {
                            var allowedByDayIndexes = new HashSet<int> { -1, 1, 2, 3, 4 };

                            if (!allowedByDayIndexes.Contains(byDayIndexResult))
                            {
                                errorMessage = "By Day invalid";

                                return;
                            }

                            byDayIndexValue = byDayIndexResult;
                        }
                        else
                        {
                            errorMessage = "By Day invalid";

                            return;
                        }
                    }

                    frequency = Frequency.Monthly;
                    byDayIndex = byDayIndexValue;
                    byDayDay = byDayDayValue;
                    byMonthDay = byMonthDayValue;
                }
                else
                {
                    errorMessage = "Recurrence Rule frequency not supported yet";

                    return;
                }

                if (frequencyString != "YEARLY")
                {
                    dictionary.TryGetValue("INTERVAL", out string? intervalString);

                    int? intervalValue = null;

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

                    if (intervalValue != null && intervalValue < 1)
                    {
                        errorMessage = "Recurrence Rule interval must be greater than zero";

                        return;
                    }

                    interval = intervalValue;
                }

                dictionary.TryGetValue("COUNT", out string? countString);
                dictionary.TryGetValue("UNTIL", out string? untilString);

                int? countValue = null;
                DateOnly? untilValue = null;

                if (countString != null && untilString != null)
                {
                    errorMessage = "Recurrence Rule must contain either 'COUNT' or 'UNTIL' but not both";

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

                if (countValue != null && countValue < 1)
                {
                    errorMessage = "Recurrence Rule count must be greater than zero";

                    return;
                }

                if (untilString != null)
                {
                    if (DateOnly.TryParseExact(untilString, "yyyyMMdd", out DateOnly untilResult))
                    {
                        untilValue = untilResult;
                    }
                    else
                    {
                        errorMessage = "Recurrence Rule until must be date only";

                        return;
                    }
                }

                count = countValue;
                until = untilValue;
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
