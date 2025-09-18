using Anatini.Server.Context;
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

        public static User AddSlug(this User user, NewUserSlug newSlug)
        {
            var userOwnedSlug = new UserOwnedSlug
            {
                SlugId = newSlug.Id,
                UserId = user.Id,
                Slug = newSlug.Slug
            };

            user.Slugs.Add(userOwnedSlug);

            if (newSlug.Default ?? false)
            {
                user.DefaultSlugId = newSlug.Id;
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
    }
}
