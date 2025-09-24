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
                Name = channel.Name
            };

            var channels = user.Channels ?? [];
            channels.Add(userOwnedChannel);
            user.Channels = channels;

            return user;
        }

        public static User AddSlug(this User user, NewUserSlug newUserSlug)
        {
            var userOwnedSlug = new UserOwnedSlug
            {
                Id = newUserSlug.Id,
                UserId = user.Id,
                Slug = newUserSlug.Slug
            };

            user.Slugs.Add(userOwnedSlug);

            if (newUserSlug.Default ?? false)
            {
                user.DefaultSlugId = newUserSlug.Id;
            }

            return user;
        }

        public static User AddSession(this User user, string refreshToken, EventData eventData)
        {
            var userSession = new UserOwnedSession
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RefreshToken = refreshToken,
                CreatedDateUtc = eventData.DateTimeUtc,
                UpdatedDateUtc = eventData.DateTimeUtc,
                IPAddress = eventData.Get("IPAddress"),
                UserAgent = eventData.Get("UserAgent"),
                Revoked = false
            };

            var sessions = user.Sessions.ToList();
            sessions.Add(userSession);
            user.Sessions = sessions;

            return user;
        }

        public static User AddInvite(this User user, Guid inviteId, string inviteCode, EventData eventData)
        {
            var userOwnedInvite = new UserOwnedInvite
            {
                Id = inviteId,
                UserId = user.Id,
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
                Id = user.Id,
                Name = user.Name,
                Emails = user.Emails.Select(ToUserEditEmailDto),
                Sessions = user.Sessions.Select(ToUserEditSessionDto),
                Slugs = user.Slugs.Select(ToUserEditSlugDto),
                Channels = user.Channels?.Select(ToUserEditChannelDto),
                Invites = user.Invites?.Select(ToUserEditInviteDto),
                DefaultSlugId = user.DefaultSlugId
            };
        }

        public static UserEditEmailDto ToUserEditEmailDto(this UserOwnedEmail userOwnedEmail)
        {
            return new UserEditEmailDto
            {
                Address = userOwnedEmail.Address,
                Id = userOwnedEmail.Id,
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
                CreatedDateUtc = userOwnedSession.CreatedDateUtc,
                UpdatedDateUtc = userOwnedSession.UpdatedDateUtc,
                IPAddress = userOwnedSession.IPAddress
            };
        }

        public static UserEditSlugDto ToUserEditSlugDto(this UserOwnedSlug userOwnedSlug)
        {
            return new UserEditSlugDto
            {
                Id = userOwnedSlug.Id,
                Slug = userOwnedSlug.Slug
            };
        }

        public static UserEditChannelDto ToUserEditChannelDto(this UserOwnedChannel userOwnedChannel)
        {
            return new UserEditChannelDto
            {
                Id = userOwnedChannel.Id,
                Name = userOwnedChannel.Name
            };
        }

        public static UserEditInviteDto ToUserEditInviteDto(this UserOwnedInvite userOwnedInvite)
        {
            return new UserEditInviteDto
            {
                Id = userOwnedInvite.Id,
                Code = userOwnedInvite.Code,
                DateNZ = userOwnedInvite.DateNZ,
                Used = userOwnedInvite.Used
            };
        }
    }
}
