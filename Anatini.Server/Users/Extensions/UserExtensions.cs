using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Utils;

namespace Anatini.Server.Users.Extensions
{
    public static class UserExtensions
    {
        public static User AddPermission(this User user, UserPermission userPermission)
        {
            var permissions = user.Permissions ?? [];

            if (permissions.Any(permission => permission == Enum.GetName(userPermission)!))
            {
                return user;
            }

            permissions.Add(Enum.GetName(userPermission)!);
            user.Permissions = permissions;

            return user;
        }

        public static bool HasAnyPermission(this User user, params UserPermission[] permissions)
        {
            return permissions.Any(permission => user.Permissions?.Any(userPermission => userPermission == Enum.GetName(permission)!) ?? false);
        }

        public static User AddChannel(this User user, Channel channel)
        {
            var userOwnedChannel = new UserOwnedChannel
            {
                Id = channel.Id,
                UserId = user.Id,
                Name = channel.Name,
                About = channel.About,
                DefaultHandle = channel.DefaultHandle
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
                Handle = userAlias.Handle,
                UserId = user.Id
            };

            user.Aliases.Add(userOwnedAlias);

            if (@default ?? false)
            {
                user.DefaultHandle = userAlias.Handle;
            }

            return user;
        }

        public static User AddSession(this User user, string refreshToken, EventData eventData)
        {
            var userOwnedSession = new UserOwnedSession
            {
                Id = RandomHex.NextX16(),
                UserId = user.Id,
                RefreshToken = refreshToken,
                CreatedDateTimeUtc = eventData.DateTimeUtc,
                UpdatedDateTimeUtc = eventData.DateTimeUtc,
                IPAddress = eventData.Get("ipAddress"),
                UserAgent = eventData.Get("userAgent"),
                Revoked = false
            };

            var sessions = user.Sessions?.ToList() ?? [];
            sessions.Add(userOwnedSession);
            user.Sessions = sessions;

            return user;
        }

        public static User RemoveSession(this User user, string refreshToken)
        {
            var userOwnedSesion = user.Sessions?.FirstOrDefault(session => session.RefreshToken == refreshToken);

            if (userOwnedSesion != null)
            {
                user.Sessions?.Remove(userOwnedSesion);
            }

            return user;
        }

        public static async Task<UserEditDto> ToUserEditDto(this User user, AnatiniContext? context = null, IBlobService? blobService = null)
        {
            ImageDto? iconImage = null;

            if (user.IconImageId != null && context != null && blobService != null)
            {
                var userImage = await context.Context.UserImages.FindAsync(user.Id, user.IconImageId);

                if (userImage != null)
                {
                    iconImage = new ImageDto
                    {
                        Uri = await blobService.GenerateUserImageLink(userImage.BlobContainerName, userImage.BlobName),
                        AltText = userImage.AltText,
                    };
                }
            }

            return new UserEditDto
            {
                Id = user.Id,
                Name = user.Name,
                About = user.About,
                Emails = user.Emails.Select(ToUserEditEmailDto),
                Sessions = user.Sessions?.Select(ToUserEditSessionDto),
                Aliases = user.Aliases.Select(ToUserEditAliasDto),
                Channels = user.Channels?.Select(ToUserEditChannelDto),
                Permissions = user.Permissions,
                DefaultHandle = user.DefaultHandle,
                Protected = user.Protected,
                IconImage = iconImage
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
                Handle = userOwnedAlias.Handle
            };
        }

        public static UserEditChannelDto ToUserEditChannelDto(this UserOwnedChannel userOwnedChannel)
        {
            return new UserEditChannelDto
            {
                Id = userOwnedChannel.Id,
                Name = userOwnedChannel.Name,
                About = userOwnedChannel.About,
                DefaultHandle = userOwnedChannel.DefaultHandle
            };
        }
    }
}
