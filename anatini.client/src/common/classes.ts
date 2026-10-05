import type { SearchParameter } from "./types";

export class Filters {
  bookmarked: string;
  starred: string;
  dismissed: string;
  followed: string;
  currentSearchParameters: SearchParameter[];

  constructor() {
    this.bookmarked = 'all';
    this.starred = 'all';
    this.dismissed = 'all';
    this.followed = 'all';
    this.currentSearchParameters = [];
  }

  getSearchParameters(): SearchParameter[] {
    this.currentSearchParameters = [];

    if (this.bookmarked !== 'all') {
      this.currentSearchParameters.push({ key: 'bookmarked', value: this.bookmarked });
    }

    if (this.starred !== 'all') {
      this.currentSearchParameters.push({ key: 'starred', value: this.starred });
    }

    if (this.dismissed !== 'all') {
      this.currentSearchParameters.push({ key: 'dismissed', value: this.dismissed });
    }

    if (this.followed !== 'all') {
      this.currentSearchParameters.push({ key: 'followed', value: this.followed });
    }

    return this.currentSearchParameters;
  }
}
