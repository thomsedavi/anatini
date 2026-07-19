import type { RouteRecordNameGeneric } from "vue-router";

export type APIResponse<T> = { fetching?: boolean, data?: T, error?: ErrorMessage };
export type SearchParameter = { key: string, value: string };
export type ErrorMessage = { heading: string, body: string };
export type StatusActions = { [id: number]: (response?: Response) => void };
export type InputError = { id: string; message: string; };
export type Status = 'idle' | 'pending' | 'success' | 'error';
export type Visibility = 'Public' | 'Protected' | 'Private';

export type Request = {
  input: RequestInfo | URL,
  statusActions: StatusActions,
  init?: RequestInit,
  searchParameters?: SearchParameter[],
  onfinally?: () => void,
};

export type SelectOption = {
  text: string;
  value: string;
  disabled?: boolean;
};

export type Image = {
  uri: string;
  altText: string | null;
};

export type User = {
  id: string;
  name: string;
  iconImage: Image | null;
  handle: string;
  about: string | null;
  hasTrusted: boolean | null;
  hasFollowed: boolean | null;
};

export type UserHeader = {
  name: string;
  iconImage: Image | null;
  handle: string;
};

export type UserEdit = {
  id: string;
  name: string;
  about: string | null;
  handle: string;
  spaces: Space[] | null;
  visibility: Visibility;
  iconImage: Image | null;
};

export type Events = {
  events: {
    type: string;
    dateTimeUtc: string;
  }[];
};

export type Space = {
  id: string;
  name: string;
  handle: string;
  about: string | null;
  visiblity: Visibility;
};

export type SpaceHeader = {
  name: string;
  handle: string;
  iconImage: Image | null;
};

export type Tab = {
  id: string;
  text: string;
  name: RouteRecordNameGeneric;
  childNames?: RouteRecordNameGeneric[];
}

export type DraftPost = {
  id: string;
  defaultHandle: string;
  name: string;
  updatedDateTimeUTC: string;
}

export type SpaceEdit = {
  id: string;
  name: string;
  about: string | null;
  handle: string;
  visiblity: Visibility;
  iconImage: Image | null;
};

export type PostVersion = {
  name: string;
  article: string;
  dateNZ: string;
}

export type PostEdit = {
  id: string;
  spaceId: string;
  defaultHandle: string;
  version: PostVersion;
  protected: boolean | null;
  status: 'Draft' | 'Published';
}

export type Post = {
  version: PostVersion;
}

export type Note = {
  id: string;
  userHeader: UserHeader | null;
  spaceHeader: SpaceHeader | null;
  handle: string | null;
  article: string;
  publishedAtUtc: string;
  hasStarred: boolean | null;
  hasBookmarked: boolean | null;
  hasDismissed: boolean | null;
}

export type NoteEdit = {
  id: string;
  handle: string | null;
  article: string;
  visibility: Visibility;
  publishedAtUtc: string;
};

export type IsAuthenticated = {
  isAuthenticated: boolean;
  isTrusted: boolean | null;
  spaces: SpaceEdit[] | null;
}
