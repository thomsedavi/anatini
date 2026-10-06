import type { SearchParameter } from "./types";

export interface Filters {
  bookmarked: string;
  starred: string;
  dismissed: string;
  followed: string;
}

export const defaultFilters: Filters = {
    bookmarked: 'all',
    starred: 'all',
    dismissed: 'all',
    followed: 'all',
}

export function getSearchParameters(filters: Filters): SearchParameter[] {
    const currentSearchParameters: SearchParameter[] = [];

    if (filters.bookmarked !== 'all') {
      currentSearchParameters.push({ key: 'bookmarked', value: filters.bookmarked });
    }

    if (filters.starred !== 'all') {
      currentSearchParameters.push({ key: 'starred', value: filters.starred });
    }

    if (filters.dismissed !== 'all') {
      currentSearchParameters.push({ key: 'dismissed', value: filters.dismissed });
    }

    if (filters.followed !== 'all') {
      currentSearchParameters.push({ key: 'followed', value: filters.followed });
    }

    return currentSearchParameters;
  }
  