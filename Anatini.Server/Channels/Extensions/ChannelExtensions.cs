using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Images.Services;

namespace Anatini.Server.Channels.Extensions
{
    public static class ChannelExtensions
    {
        public static ChannelDto ToChannelDto(this Channel channel)
        {
            return new ChannelDto
            {
                Id = channel.Id,
                Name = channel.Name,
                About = channel.About,
                Handle = channel.Handle
            };
        }

        public static async Task<ChannelHeaderDto> ToChannelHeaderDto(this Channel channel, IBlobService? blobService = null)
        {
            return new ChannelHeaderDto
            {
                Name = channel.Name,
                Handle = channel.Handle,
                IconImage = await channel.GetIconImageAsync(blobService)
            };
        }

        public static async Task<ChannelEditDto> ToChannelEditDtoAsync(this Channel channel, IBlobService? blobService = null)
        {
            return new ChannelEditDto
            {
                Id = channel.Id,
                Name = channel.Name,
                About = channel.About,
                Handle = channel.Handle,
                Visibility = channel.Visibility.ToString(),
                IconImage = await channel.GetIconImageAsync(blobService)
            };
        }

        private static async Task<ImageDto?> GetIconImageAsync(this Channel channel, IBlobService? blobService = null)
        {
            ImageDto? iconImage = null;

            if (blobService != null)
            {
                var userIconImage = channel.Images.FirstOrDefault(image => image.Handle == "icon");

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
