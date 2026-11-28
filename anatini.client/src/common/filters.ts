import type { SearchParameter } from "./types";

export interface Filters {
  bookmarked: string;
  liked: string;
  hidden: string;
  followed: string;
}

export const defaultFilters: Filters = {
    bookmarked: 'all',
    liked: 'all',
    hidden: 'all',
    followed: 'all',
}

export function getSearchParameters(filters: Filters): SearchParameter[] {
    const currentSearchParameters: SearchParameter[] = [];

    if (filters.bookmarked !== 'all') {
      currentSearchParameters.push({ key: 'bookmarked', value: filters.bookmarked });
    }

    if (filters.liked !== 'all') {
      currentSearchParameters.push({ key: 'liked', value: filters.liked });
    }

    if (filters.hidden !== 'all') {
      currentSearchParameters.push({ key: 'hidden', value: filters.hidden });
    }

    if (filters.followed !== 'all') {
      currentSearchParameters.push({ key: 'followed', value: filters.followed });
    }

    return currentSearchParameters;
  }
  