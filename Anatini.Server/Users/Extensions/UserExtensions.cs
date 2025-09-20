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
                ChannelId = channel.Id,
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
                SlugId = newUserSlug.Id,
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
                SessionId = Guid.NewGuid(),
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
                InviteId = inviteId,
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

        public static AccountDto ToAccountDto(this User user)
        {
            var accountDto = new AccountDto
            {
                Id = user.Id,
                Name = user.Name,
                Emails = user.Emails.Select(ToAccountEmailDto),
                Sessions = user.Sessions.Select(ToAccountSessionDto),
                Slugs = user.Slugs.Select(ToAccountSlugDto),
                Channels = user.Channels?.Select(ToAccountChannelDto),
                Invites = user.Invites?.Select(ToAccountInviteDto),
                DefaultSlugId = user.DefaultSlugId
            };

            return accountDto;
        }

        public static AccountEmailDto ToAccountEmailDto(this UserOwnedEmail userOwnedEmail)
        {
            var accountEmailDto = new AccountEmailDto
            {
                Address = userOwnedEmail.Address,
                EmaiId = userOwnedEmail.EmailId,
                Verified = userOwnedEmail.Verified
            };

            return accountEmailDto;
        }

        public static AccountSessionDto ToAccountSessionDto(this UserOwnedSession userOwnedSession)
        {
            var accountSessionDto = new AccountSessionDto
            {
                UserAgent = userOwnedSession.UserAgent,
                Revoked = userOwnedSession.Revoked,
                CreatedDateUtc = userOwnedSession.CreatedDateUtc,
                UpdatedDateUtc = userOwnedSession.UpdatedDateUtc,
                IPAddress = userOwnedSession.IPAddress
            };

            return accountSessionDto;
        }

        public static AccountSlugDto ToAccountSlugDto(this UserOwnedSlug userOwnedSlug)
        {
            var accountSlugDto = new AccountSlugDto
            {
                SlugId = userOwnedSlug.SlugId,
                Slug = userOwnedSlug.Slug
            };

            return accountSlugDto;
        }

        public static AccountChannelDto ToAccountChannelDto(this UserOwnedChannel userOwnedChannel)
        {
            var accountChannelDto = new AccountChannelDto
            {
                ChannelId = userOwnedChannel.ChannelId,
                Name = userOwnedChannel.Name
            };

            return accountChannelDto;
        }

        public static AccountInviteDto ToAccountInviteDto(this UserOwnedInvite userOwnedInvite)
        {
            var accountInviteDto = new AccountInviteDto
            {
                InviteId = userOwnedInvite.InviteId,
                Code = userOwnedInvite.Code,
                DateNZ = userOwnedInvite.DateNZ,
                Used = userOwnedInvite.Used
            };

            return accountInviteDto;
        }
    }
}
