using Anatini.Server.Context;
using Anatini.Server.Dtos;
using Anatini.Server.Utils;

namespace Anatini.Server.Users.Extensions
{
    public static class UserExtensions
    {
        public static User AddChannel(this User user, Channel channel)
        {
            var userOwnedChannel = new UserOwnedChannel
            {
                Id = channel.Id,
                UserId = user.Id,
                Name = channel.Name,
                DefaultSlug = channel.DefaultSlug
            };

            var channels = user.Channels ?? [];
            channels.Add(userOwnedChannel);
            user.Channels = channels;

            return user;
        }

        public static User AddAlias(this User user, UserAlias userAlias, bool? @default)
        {
            var userOwnedAlias = new UserOwnedAlias
            {
                Slug = userAlias.Slug,
                UserId = user.Id
            };

            user.Aliases.Add(userOwnedAlias);

            if (@default ?? false)
            {
                user.DefaultSlug = userAlias.Slug;
            }

            return user;
        }

        public static User AddSession(this User user, string refreshToken, EventData eventData)
        {
            var userOwnedSession = new UserOwnedSession
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RefreshToken = refreshToken,
                CreatedDateTimeUtc = eventData.DateTimeUtc,
                UpdatedDateTimeUtc = eventData.DateTimeUtc,
                IPAddress = eventData.Get("ipAddress"),
                UserAgent = eventData.Get("userAgent"),
                Revoked = false
            };

            var sessions = user.Sessions.ToList();
            sessions.Add(userOwnedSession);
            user.Sessions = sessions;

            return user;
        }

        public static User AddInvite(this User user, string inviteCode, EventData eventData)
        {
            var userOwnedInvite = new UserOwnedInvite
            {
                Code = inviteCode,
                UserId = user.Id,
                Used = false,
                DateOnlyNZ = eventData.DateOnlyNZNow
            };

            var invites = user.Invites ?? [];
            invites.Add(userOwnedInvite);
            user.Invites = invites;

            return user;
        }

        public static UserEditDto ToUserEditDto(this User user)
        {
            return new UserEditDto
            {
                Id = user.Id,
                Name = user.Name,
                Emails = user.Emails.Select(ToUserEditEmailDto),
                Sessions = user.Sessions.Select(ToUserEditSessionDto),
                Aliases = user.Aliases.Select(ToUserEditAliasDto),
                Channels = user.Channels?.Select(ToUserEditChannelDto),
                Invites = user.Invites?.Select(ToUserEditInviteDto),
                DefaultSlug = user.DefaultSlug
            };
        }

        public static UserEditEmailDto ToUserEditEmailDto(this UserOwnedEmail userOwnedEmail)
        {
            return new UserEditEmailDto
            {
                Address = userOwnedEmail.Address,
                Verified = userOwnedEmail.Verified
            };
        }

        public static UserEditSessionDto ToUserEditSessionDto(this UserOwnedSession userOwnedSession)
        {
            return new UserEditSessionDto
            {
                Id = userOwnedSession.Id,
                UserAgent = userOwnedSession.UserAgent,
                Revoked = userOwnedSession.Revoked,
                CreatedDateTimeUtc = userOwnedSession.CreatedDateTimeUtc,
                UpdatedDateTimeUtc = userOwnedSession.UpdatedDateTimeUtc,
                IPAddress = userOwnedSession.IPAddress
            };
        }

        public static UserEditAliasDto ToUserEditAliasDto(this UserOwnedAlias userOwnedAlias)
        {
            return new UserEditAliasDto
            {
                Slug = userOwnedAlias.Slug
            };
        }

        public static UserEditChannelDto ToUserEditChannelDto(this UserOwnedChannel userOwnedChannel)
        {
            return new UserEditChannelDto
            {
                Id = userOwnedChannel.Id,
                Name = userOwnedChannel.Name,
                DefaultSlug = userOwnedChannel.DefaultSlug
            };
        }

        public static UserEditInviteDto ToUserEditInviteDto(this UserOwnedInvite userOwnedInvite)
        {
            return new UserEditInviteDto
            {
                Code = userOwnedInvite.Code,
                DateOnlyNZ = userOwnedInvite.DateOnlyNZ,
                Used = userOwnedInvite.Used
            };
        }
    }
}
