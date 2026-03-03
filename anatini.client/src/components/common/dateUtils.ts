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

export function formatUTC(value: string): string {
    return new Date(value).toISOString().split('.')[0] + 'Z';
}
