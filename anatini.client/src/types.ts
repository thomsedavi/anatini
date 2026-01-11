export type ErrorMessage = { error: true, heading: string, body: string };
export type StatusActions = { [id: number]: (response?: Response) => void };
export type InputError = { id: string; message: string; };
export type Status = 'idle' | 'pending' | 'success' | 'error';

export type Image = {
  uri: string;
  altText: string | null;
};

export type User = {
  id: string;
  name: string;
  iconImage: Image | null;
  bio: string | null;
};

export type UserEdit = {
  id: string;
  name: string;
  bio: string | null;
  defaultSlug: string;
  emails: {
    address: string;
    verified: boolean;
  }[];
  channels: Channel[] | null;
  sessions: {
    id: string;
    userAgent: string;
    revoked: boolean;
    createdDateTimeUtc: string;
    updatedDateTimeUtc: string;
    ipAddress: string;
  }[];
  aliases: {
    slug: string;
  }[];
  permissions: string[] | null;
  protected: boolean | null;
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
  defaultSlug: string;
  topContents: {
    defaultSlug: string;
    name: string;
  }[] | null;
};

export type DraftContent = {
  id: string;
  defaultSlug: string;
  name: string;
  updatedDateTimeUTC: string;
}

export type ChannelEdit = {
  id: string;
  name: string;
  defaultSlug: string;
  aliases: {
    slug: string;
  }[];
  topDraftContents: DraftContent[] | null;
};

export type ContentElement = {
  index: number;
  tag: string;
  content: string | null;
}

export type ContentVersion = {
  name: string;
  elements: ContentElement[] | null;
  dateNZ: string;
}

export type ContentEdit = {
  id: string;
  channelId: string;
  defaultSlug: string;
  version: ContentVersion;
  protected: boolean | null;
}

export type Content = {
  version: ContentVersion;
}

export type IsAuthenticated = {
  isAuthenticated: boolean;
  expiresUtc: string | null;
}
