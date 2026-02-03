using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Images.Services;

namespace Anatini.Server.Users.Extensions
{
    public static class UserSlugExtensions
    {
        public static async Task<UserDto> ToUserDto(this UserAlias userAlias, AnatiniContext context, IBlobService blobService)
        {
            ImageDto? iconImage = null;

            if (userAlias.IconImageId != null)
            {
                var userImage = await context.Context.UserImages.FindAsync(userAlias.UserId, userAlias.IconImageId);

                if (userImage != null)
                {
                    iconImage = new ImageDto
                    {
                        Uri = await blobService.GenerateUserImageLink(userImage.BlobContainerName, userImage.BlobName),
                        AltText = userImage.AltText,
                    };
                }
            }

            return new UserDto
            {
                Id = userAlias.UserId,
                Name = userAlias.UserName,
                About = userAlias.UserAbout,
                IconImage = iconImage
            };
        }
    }
}
