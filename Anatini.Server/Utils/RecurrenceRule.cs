using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;

namespace Anatini.Server.Utils
{
    public class RecurrenceRule
    {
        private readonly RecurrencePattern? recurrencePattern;
        private readonly string? errorMessage;

        public RecurrenceRule(string? recurrenceRule)
        {
            if (recurrenceRule != null)
            {
                try
                {
                    recurrencePattern = new RecurrencePattern(recurrenceRule);
                }
                catch(Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }
        }

        public string? ErrorMessage => errorMessage;

        public IEnumerable<(DateTime, DateTime)> GetOccurrence(string timeZoneId, DateTime start, DateTime? end, TimeSpan? duration)
        {
            var result = new List<(DateTime, DateTime)>();

            if (recurrencePattern != null && (end.HasValue || duration.HasValue))
            {
                var occurrencesNz = GetOccurrences(timeZoneId, start, end, duration);

                foreach (var occurrenceNz in occurrencesNz)
                {
                    if (occurrenceNz.Period.EffectiveEndTime != null)
                    {
                        result.Add(new(occurrenceNz.Period.StartTime.Value, occurrenceNz.Period.EffectiveEndTime.Value));
                    }
                }

            }

            return result;
        }

        public DateTime? ExpiresAt(string timeZoneId, DateTime start, DateTime? end, TimeSpan? duration)
        {
            if (recurrencePattern != null && (end.HasValue || duration.HasValue))
            {
                var lastOccurrenceNz = GetOccurrences(timeZoneId, start, end, duration).LastOrDefault();

                if (lastOccurrenceNz != null)
                {
                    return lastOccurrenceNz.Period.EffectiveEndTime?.Value;
                }
            }

            return null;
        }

        private IEnumerable<Occurrence> GetOccurrences(string timeZoneId, DateTime start, DateTime? end, TimeSpan? duration)
        {
            var calendarEvent = new CalendarEvent
            {
                Start = new CalDateTime(start, timeZoneId),
                RecurrenceRule = recurrencePattern
            };

            if (end.HasValue)
            {
                calendarEvent.End = new CalDateTime(end.Value, timeZoneId);
            }
            else if (duration.HasValue)
            {
                calendarEvent.Duration = Duration.FromTimeSpanExact(duration.Value);
            }

            return calendarEvent.GetOccurrences();
        }
    }
}
