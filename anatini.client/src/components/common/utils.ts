export function tidy(text: string): string {
    return text.trim().replace(/\s+/g, ' ');
}
