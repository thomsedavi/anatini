// March 3, 2026
const dateformatter = new Intl.DateTimeFormat('en-NZ', {
  dateStyle: 'long'
});

const dateTimeFormatter = new Intl.DateTimeFormat('en-NZ', {
  day: 'numeric',
  month: 'long',
  year: 'numeric',
  hour: 'numeric',
  minute: 'numeric'
});

const dateToHourFormatter = new Intl.DateTimeFormat('en-NZ', {
  day: 'numeric',
  month: 'long',
  year: 'numeric',
  hour: 'numeric',
});

const dateToDayFormatter = new Intl.DateTimeFormat('en-NZ', {
  day: 'numeric',
  month: 'long',
  year: 'numeric',
});

const dateToMonthFormatter = new Intl.DateTimeFormat('en-NZ', {
  month: 'long',
  year: 'numeric',
});

const dateToYearFormatter = new Intl.DateTimeFormat('en-NZ', {
  year: 'numeric',
});

const timeFormatter = new Intl.DateTimeFormat('en-NZ', {
  timeStyle: 'short'
});

const weekdayFormatter = new Intl.DateTimeFormat('en-NZ', {
  weekday: 'long'
});

export function formatShortestDateTime(value: string): string {
  const date = new Date(value);

  if (value.slice(14, 16) === '00') {
    if (value.slice(11, 13) === '00') {
      if (value.slice(8, 10) === '01') {
        if (value.slice(5, 7) === '01') {
          return dateToYearFormatter.format(date);
        }

        return dateToMonthFormatter.format(date);
      }

      return dateToDayFormatter.format(date);
    }

    return dateToHourFormatter.format(date);
  }

  return dateTimeFormatter.format(date);
}

export function formatDate(value: string): string {
  return dateformatter.format(new Date(value));
}

export function formatDateTime(value: string): string {
  return dateTimeFormatter.format(new Date(value));
}

export function formatTime(value: string): string {
  return timeFormatter.format(new Date(value));
}

export function getDayOfWeek(value: string): string {
  return weekdayFormatter.format(new Date(value));
}

export function getDaysDifference(value1: string, value2: string): number {
  const [y1, m1, d1] = value1.slice(0, 10).split('-').map(Number);
  const [y2, m2, d2] = value2.slice(0, 10).split('-').map(Number);

  const time1 = Date.UTC(y1!, m1! - 1, d1);
  const time2 = Date.UTC(y2!, m2! - 1, d2);

  const oneDayInMs = 24 * 60 * 60 * 1000;

  return Math.abs(time2 - time1) / oneDayInMs;
}

export function formatDateTimeNz(value: string): string {
  const date = new Date(value);

  const parts = new Intl.DateTimeFormat('en-CA', {
    timeZone: 'Pacific/Auckland',
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    hour12: false
  }).formatToParts(date);

  const partsMap = Object.fromEntries(parts.map(part => [part.type, part.value]));

  return `${partsMap.year}-${partsMap.month}-${partsMap.day}T${partsMap.hour}:${partsMap.minute}`;
}
