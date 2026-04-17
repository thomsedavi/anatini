using Anatini.Server.Channels.Extensions;
using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Images.Services;

namespace Anatini.Server.Users.Extensions
{
    public static class UserExtensions
    {
        public static async Task<UserEditDto> ToUserEditDto(this ApplicationUser user, IBlobService? blobService = null)
        {
            ImageDto? iconImage = null;

            if (blobService != null)
            {
                var userIconImage = user.Images.FirstOrDefault(image => image.Handle == "icon");

                if (userIconImage != null)
                {
                    iconImage = new ImageDto
                    {
                        Uri = await blobService.GenerateUserImageLink(userIconImage.BlobContainerName, userIconImage.BlobName),
                        AltText = userIconImage.AltText,
                    };
                }
            }

            return new UserEditDto
            {
                Id = user.Id,
                Name = user.Name,
                About = user.About,
                Channels = user.UserChannels.Select(userChannel => userChannel.Channel.ToChannelEditDto()),
                Handle = user.Handle,
                UserName = user.UserName,
                Visibility = user.Visibility.ToString(),
                IconImage = iconImage
            };
        }
    }
}
