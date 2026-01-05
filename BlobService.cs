using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ABCRetail_Cloud_Services
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _containerClient;
        private readonly string _containerName = "products";

        public BlobService(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        }

        // Initialize container (create if not exists)
        public async Task InitializeAsync()
        {
            await _containerClient.CreateIfNotExistsAsync();
        }

        // Upload a file to blob storage
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            await InitializeAsync();

            BlobClient blobClient = _containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, overwrite: true);

            return blobClient.Uri.ToString();
        }

        // Download a file from blob storage
        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(fileName);

            if (await blobClient.ExistsAsync())
            {
                var response = await blobClient.DownloadAsync();
                return response.Value.Content;
            }

            throw new FileNotFoundException($"Blob {fileName} not found in container {_containerName}");
        }

        // Delete a file from blob storage
        public async Task<bool> DeleteFileAsync(string fileName)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(fileName);
            return await blobClient.DeleteIfExistsAsync();
        }

        // Check if file exists
        public async Task<bool> FileExistsAsync(string fileName)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(fileName);
            return await blobClient.ExistsAsync();
        }

        // Get file URL
        public string GetFileUrl(string fileName)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(fileName);
            return blobClient.Uri.ToString();
        }
    }
}

