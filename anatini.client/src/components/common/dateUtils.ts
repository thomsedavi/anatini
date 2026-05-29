// March 3, 2026
const shortformatter = new Intl.DateTimeFormat('en-NZ', {
  dateStyle: 'long'
});

const longFormatter = new Intl.DateTimeFormat('en-NZ', {
  dateStyle: 'long',
  timeStyle: 'short'
});

export function formatShort(value: string): string {
  return shortformatter.format(new Date(value));
}

export function formatLong(value: string): string {
  return longFormatter.format(new Date(value));
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

export function formatUTC(value: string): string {
    return new Date(value).toISOString().split('.')[0] + 'Z';
}
