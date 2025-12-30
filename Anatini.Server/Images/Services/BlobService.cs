using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Caching.Memory;

namespace Anatini.Server.Images.Services
{
    public interface IBlobService
    {
        Task<Response<BlobContentInfo>> UploadAsync(IFormFile file, string blobContainerName, string blobName);
        Task<string> GenerateUserImageLink(string blobContainerName, string blobName);
    }

    public class BlobService(BlobServiceClient blobServiceClient, IMemoryCache cache) : IBlobService
    {
        private readonly IMemoryCache _cache = cache;
        private static readonly SemaphoreSlim _lock = new(1, 1);
        private const string CacheKey = "UserDelegationKey";

        public async Task<Response<BlobContentInfo>> UploadAsync(IFormFile file, string blobContainerName, string blobName)
        {
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);

            var blobClient = blobContainerClient.GetBlobClient(blobName);

            using var content = file.OpenReadStream();

            return await blobClient.UploadAsync(content, true);
        }

        public async Task<UserDelegationKey> GetCachedKeyAsync()
        {
            if (_cache.TryGetValue(CacheKey, out UserDelegationKey? key) && key != null)
            {
                return key;
            }

            await _lock.WaitAsync();

            try
            {
                if (!_cache.TryGetValue(CacheKey, out key) || key == null)
                {
                    var startTime = DateTimeOffset.UtcNow.AddMinutes(-15);
                    var expiryTime = startTime.AddDays(6);

                    key = await blobServiceClient.GetUserDelegationKeyAsync(startTime, expiryTime);

                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(expiryTime.AddMinutes(-30));

                    _cache.Set(CacheKey, key, cacheOptions);
                }
                return key;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<string> GenerateUserImageLink(string blobContainerName, string blobName)
        {
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);

            var blobClient = blobContainerClient.GetBlobClient(blobName);

            var delegationKey = await GetCachedKeyAsync();

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = blobContainerName,
                BlobName = blobName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-15),
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateUserDelegationSasUri(sasBuilder, delegationKey).ToString();
        }
    }
}
