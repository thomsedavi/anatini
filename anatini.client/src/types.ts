export type User = {
  name: string;
};

export type UserEdit = {
  id: string,
  name: string;
  defaultSlug: string;
  emails: {
    address: string;
    verified: boolean;
  }[];
  channels?: {
    id: string;
    name: string;
    defaultSlug: string;
  }[];
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
};

export type Events = {
  events: {
    type: string;
    dateTimeUtc: string;
  }[];
};

export type Channel = {
  name: string;
  slug: string;
  topContents?: {
    defaultSlug: string;
    name: string;
  }[];
};

export type ChannelEdit = {
  id: string;
  name: string;
  defaultSlug: string;
  aliases: {
    slug: string;
  }[];
  topDraftContents?: {
    id: string;
    defaultSlug: string;
    name: string;
    updatedDateTimeUTC: string;
  }[];
};

export type ContentElement = {
  index: number;
  tag: string;
  content: string | null;
}

export type ContentVersion = {
  name: string;
  elements?: ContentElement[];
}

export type ContentEdit = {
  id: string;
  channelId: string;
  defaultSlug: string;
  dateNZ: string;
  version: ContentVersion;
}

export type Content = {
  version: ContentVersion;
}

export type IsAuthenticated = {
  isAuthenticated: boolean;
  expiresUtc?: string;
}
