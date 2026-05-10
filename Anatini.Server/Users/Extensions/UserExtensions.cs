using Anatini.Server.Channels.Extensions;
using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Images.Services;

namespace Anatini.Server.Users.Extensions
{
    public static class UserExtensions
    {
        public static async Task<UserDto> ToUserDtoAsync(this ApplicationUser user, IBlobService? blobService = null)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                About = user.About,
                Handle = user.Handle,
                IconImage = await user.GetIconImageAsync(blobService)
            };
        }

        public static async Task<UserHeaderDto> ToUserHeaderDtoAsync(this ApplicationUser user, IBlobService? blobService = null)
        {
            return new UserHeaderDto
            {
                Name = user.Name,
                Handle = user.Handle,
                IconImage = await user.GetIconImageAsync(blobService)
            };
        }

        public static async Task<UserEditDto> ToUserEditDtoAsync(this ApplicationUser user, IBlobService? blobService = null)
        {
            return new UserEditDto
            {
                Id = user.Id,
                Name = user.Name,
                About = user.About,
                Channels = await Task.WhenAll(user.ChannelEdges.Select(userChannelEdge => userChannelEdge.Channel.ToChannelEditDtoAsync(blobService))),
                Handle = user.Handle,
                UserName = user.UserName,
                Visibility = user.Visibility.ToString(),
                IconImage = await user.GetIconImageAsync(blobService)
            };
        }

        private static async Task<ImageDto?> GetIconImageAsync(this ApplicationUser user, IBlobService? blobService = null)
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

            return iconImage;
        }
    }
}
