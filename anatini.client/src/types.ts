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
  invites?: {
    code: string;
    dateOnlyNZ: string;
    used: boolean;
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

export type Post = {
  name: string;
}

export type Channel = {
  name: string;
  slug: string;
  topPosts?: {
    defaultSlug: string;
    name: string;
  }[];
};

export  type ChannelEdit = {
  id: string;
  name: string;
  defaultSlug: string;
  aliases: {
    slug: string;
  }[];
  topDraftPosts?: {
    id: string;
    defaultSlug: string;
    name: string;
    updatedDateTimeUTC: string;
  }[];
};
