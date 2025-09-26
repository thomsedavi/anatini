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
                Guid = channel.Guid,
                UserGuid = user.Guid,
                Name = channel.Name
            };

            var channels = user.Channels ?? [];
            channels.Add(userOwnedChannel);
            user.Channels = channels;

            return user;
        }

        public static User AddAlias(this User user, NewUserAlias newUserAlias)
        {
            var userOwnedAlias = new UserOwnedAlias
            {
                Guid = newUserAlias.Guid,
                UserGuid = user.Guid,
                Slug = newUserAlias.Slug
            };

            user.Aliases.Add(userOwnedAlias);

            if (newUserAlias.Default ?? false)
            {
                user.DefaultAliasGuid = newUserAlias.Guid;
            }

            return user;
        }

        public static User AddSession(this User user, string refreshToken, EventData eventData)
        {
            var userOwnedSession = new UserOwnedSession
            {
                Guid = Guid.NewGuid(),
                UserGuid = user.Guid,
                RefreshToken = refreshToken,
                CreatedDateUtc = eventData.DateTimeUtc,
                UpdatedDateUtc = eventData.DateTimeUtc,
                IPAddress = eventData.Get("IPAddress"),
                UserAgent = eventData.Get("UserAgent"),
                Revoked = false
            };

            var sessions = user.Sessions.ToList();
            sessions.Add(userOwnedSession);
            user.Sessions = sessions;

            return user;
        }

        public static User AddInvite(this User user, Guid inviteId, string inviteCode, EventData eventData)
        {
            var userOwnedInvite = new UserOwnedInvite
            {
                Guid = inviteId,
                UserGuid = user.Guid,
                Code = inviteCode,
                Used = false,
                DateNZ = eventData.DateOnlyNZNow
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
                Id = user.Guid,
                Name = user.Name,
                Emails = user.Emails.Select(ToUserEditEmailDto),
                Sessions = user.Sessions.Select(ToUserEditSessionDto),
                Aliases = user.Aliases.Select(ToUserEditAliasDto),
                Channels = user.Channels?.Select(ToUserEditChannelDto),
                Invites = user.Invites?.Select(ToUserEditInviteDto),
                DefaultAliasId = user.DefaultAliasGuid
            };
        }

        public static UserEditEmailDto ToUserEditEmailDto(this UserOwnedEmail userOwnedEmail)
        {
            return new UserEditEmailDto
            {
                Guid = userOwnedEmail.Guid,
                Address = userOwnedEmail.Address,
                Verified = userOwnedEmail.Verified
            };
        }

        public static UserEditSessionDto ToUserEditSessionDto(this UserOwnedSession userOwnedSession)
        {
            return new UserEditSessionDto
            {
                Guid = userOwnedSession.Guid,
                UserAgent = userOwnedSession.UserAgent,
                Revoked = userOwnedSession.Revoked,
                CreatedDateUtc = userOwnedSession.CreatedDateUtc,
                UpdatedDateUtc = userOwnedSession.UpdatedDateUtc,
                IPAddress = userOwnedSession.IPAddress
            };
        }

        public static UserEditAliasDto ToUserEditAliasDto(this UserOwnedAlias userOwnedAlias)
        {
            return new UserEditAliasDto
            {
                Guid = userOwnedAlias.Guid,
                Slug = userOwnedAlias.Slug
            };
        }

        public static UserEditChannelDto ToUserEditChannelDto(this UserOwnedChannel userOwnedChannel)
        {
            return new UserEditChannelDto
            {
                Guid = userOwnedChannel.Guid,
                Name = userOwnedChannel.Name
            };
        }

        public static UserEditInviteDto ToUserEditInviteDto(this UserOwnedInvite userOwnedInvite)
        {
            return new UserEditInviteDto
            {
                Guid = userOwnedInvite.Guid,
                Code = userOwnedInvite.Code,
                DateNZ = userOwnedInvite.DateNZ,
                Used = userOwnedInvite.Used
            };
        }
    }
}
