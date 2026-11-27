import type { RouteRecordNameGeneric } from "vue-router";

export type APIResponse<T> = { fetching?: boolean, data?: T, error?: ErrorMessage };
export type SearchParameter = { key: string, value: string };
export type ErrorMessage = { status: string, heading: string, body: string };
export type StatusActions = { [id: number]: (response?: Response) => void };
export type InputError = { id: string; message: string; };
export type Status = 'idle' | 'pending' | 'success' | 'error';
export type Visibility = 'Public' | 'Protected' | 'Private';
export type ContentType = 'Note' | 'Work' | 'EventOccurrence';
export type ContentStatus = 'Draft' | 'Published' | 'Archived';
export type OwnerType = 'User' | 'Space';

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
  active?: boolean;
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
  isTrusted: boolean | null;
  isFollowed: boolean | null;
};

export type EventHeader = {
  id: string;
  handle: string;
}

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
  settings: { [key: string]: string }
};

export type OwnerHeader = {
  id: string;
  name: string;
  handle: string;
  iconImage: Image | null;
  isFollowed: boolean | null;
  type: OwnerType;
};

export type Tab = {
  id: string;
  text: string;
  name: RouteRecordNameGeneric;
  childNames?: RouteRecordNameGeneric[];
}

export type DraftNote = {
  id: string;
  defaultHandle: string;
  title: string;
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

export type NoteVersion = {
  title: string;
  body: string;
  dateNZ: string;
}

export type NoteEdit = {
  id: string;
  spaceId: string;
  defaultHandle: string;
  version: NoteVersion;
  protected: boolean | null;
  status: 'Draft' | 'Published';
}

export type Content = {
  id: string;
  handle: string;
  body: string;
  visibility: Visibility;
  status: ContentStatus;
  isScheduled: boolean | null;
  isStarred: boolean | null;
  isBookmarked: boolean | null;
  isDismissed: boolean | null;
  isCollected: boolean | null;
  isOwned: boolean | null;
  ownerHeader: OwnerHeader | null;
}

export type Event = Content & {
  title: string;
}

export type EventOccurrence = Content & {
  title: string;
  startsAtNz: string;
  endsAtNz: string;
  eventHeader: EventHeader;
}

export type Work = Content & {
  title: string;
  publishedAtNz: string | null;
}

export type Note = Content & {
  title: string | null;
  publishedAtNz: string;
}

export type Account = {
  isAuthenticated: boolean | null;
  isTrusted: boolean | null;
  spaces: SpaceEdit[] | null;
  userId: string | null;
  userHandle: string | null;
}
