import type { RouteRecordNameGeneric } from "vue-router";

export type SearchParameter = { key: string, value: string };
export type ErrorMessage = { error: true, heading: string, body: string };
export type StatusActions = { [id: number]: (response?: Response) => void };
export type InputError = { id: string; message: string; };
export type Status = 'idle' | 'pending' | 'success' | 'error';
export type Visibility = 'Public' | 'Protected' | 'Private';

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
  channels: Channel[] | null;
  visibility: Visibility;
  iconImage: Image | null;
};

export type Events = {
  events: {
    type: string;
    dateTimeUtc: string;
  }[];
};

export type Channel = {
  id: string;
  name: string;
  handle: string;
  about: string | null;
  visiblity: Visibility;
};

export type ChannelHeader = {
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

export type ChannelEdit = {
  id: string;
  name: string;
  about: string | null;
  handle: string;
  iconImage: Image | null;
  visiblity: Visibility;
};

export type PostElement = {
  index: number;
  tag: string;
  post: string | null;
}

export type PostVersion = {
  name: string;
  article: string;
  dateNZ: string;
}

export type PostEdit = {
  id: string;
  channelId: string;
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
  channelHeader: ChannelHeader | null;
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
}
