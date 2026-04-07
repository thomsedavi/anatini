using Anatini.Server.Channels.Extensions;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Images.Services;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Users.Extensions
{
    public static class UserExtensions
    {
        public static async Task<UserEditDto> ToUserEditDto(this ApplicationUser user, ApplicationDbContext context, IBlobService? blobService = null)
        {
            ImageDto? iconImage = null;
        
            if (user.IconImageId != null && blobService != null)
            {
                var userImage = await context.UserImages.AsNoTracking().FirstOrDefaultAsync(userImage => userImage.Id == user.IconImageId);

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
                Channels = user.UserChannels.Select(userChannel => userChannel.Channel.ToChannelEditDto()),
                Handle = user.Handle,
                UserName = user.UserName,
                Visibility = user.Visibility.ToString(),
                IconImage = iconImage
            };
        }
    }
}
