using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Anatini.Server.Images.Services
{
    public interface IBlobService
    {
        Task<Response<BlobContentInfo>> UploadAsync(IFormFile file, string blobContainerName, string blobName);
    }

    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobService()
        {
            _blobServiceClient = new BlobServiceClient("TODO");
        }

        public async Task<Response<BlobContentInfo>> UploadAsync(IFormFile file, string blobContainerName, string blobName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);

            var blobClient = blobContainerClient.GetBlobClient(blobName);

            using var content = file.OpenReadStream();

            return await blobClient.UploadAsync(content, true);
        }
    }
}
