using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ABCRetail_Cloud_.Services
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _containerClient;
        private readonly string _containerName = "products";

        public BlobService(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            await _containerClient.CreateIfNotExistsAsync();
            var blobClient = _containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, overwrite: true);
            return blobClient.Uri.ToString();
        }
    }
}

