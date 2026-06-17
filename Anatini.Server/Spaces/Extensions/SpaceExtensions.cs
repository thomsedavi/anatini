using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Images.Services;

namespace Anatini.Server.Spaces.Extensions
{
    public static class SpaceExtensions
    {
        public static SpaceDto ToSpaceDto(this Space space)
        {
            return new SpaceDto
            {
                Id = space.Id,
                Name = space.Name,
                About = space.About,
                Handle = space.Handle
            };
        }

        public static async Task<SpaceHeaderDto> ToSpaceHeaderDto(this Space space, IBlobService? blobService = null)
        {
            return new SpaceHeaderDto
            {
                Name = space.Name,
                Handle = space.Handle,
                IconImage = await space.GetIconImageAsync(blobService)
            };
        }

        public static async Task<SpaceEditDto> ToSpaceEditDtoAsync(this Space space, IBlobService? blobService = null)
        {
            return new SpaceEditDto
            {
                Id = space.Id,
                Name = space.Name,
                About = space.About,
                Handle = space.Handle,
                Visibility = space.Visibility.ToString(),
                IconImage = await space.GetIconImageAsync(blobService)
            };
        }

        private static async Task<ImageDto?> GetIconImageAsync(this Space space, IBlobService? blobService = null)
        {
            ImageDto? iconImage = null;

            if (blobService != null)
            {
                var userIconImage = space.Images.FirstOrDefault(image => image.Handle == "icon");

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
