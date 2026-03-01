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
  about: string | null;
};

export type UserEdit = {
  id: string;
  name: string;
  about: string | null;
  defaultHandle: string;
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
    handle: string;
  }[];
  permissions: string[] | null;
  protected: boolean | null;
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
  about: string | null;
  defaultHandle: string;
};

export type Tab = {
  id: string;
  text: string;
  name: string;
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
  defaultHandle: string;
  aliases: {
    handle: string;
  }[];
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
  article: string;
}

export type IsAuthenticated = {
  isAuthenticated: boolean;
  expiresUtc: string | null;
}
