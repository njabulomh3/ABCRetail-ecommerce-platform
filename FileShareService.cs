    using Azure.Storage.Files.Shares;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

namespace ABCRetail_Cloud_.Services
{
        public class FileShareService
        {
            private readonly ShareServiceClient _shareServiceClient;
            private readonly string _shareName;

            public FileShareService(ShareServiceClient shareServiceClient, string shareName)
            {
                _shareServiceClient = shareServiceClient ?? throw new ArgumentNullException(nameof(shareServiceClient));
                _shareName = shareName ?? throw new ArgumentNullException(nameof(shareName));
            }

            public async Task UploadFileAsync(string fileName, Stream fileStream)
            {
                try
                {
                    var shareClient = _shareServiceClient.GetShareClient(_shareName);
                    await shareClient.CreateIfNotExistsAsync();

                    var rootDir = shareClient.GetRootDirectoryClient();
                    var fileClient = rootDir.GetFileClient(fileName);
                    await fileClient.CreateAsync(fileStream.Length);
                    await fileClient.UploadRangeAsync(new Azure.HttpRange(0, fileStream.Length), fileStream);
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred while uploading the file: {ex.Message}", ex);
                }
            }

            public async Task<Stream> DownloadFileAsync(string fileName)
            {
                try
                {
                    var shareClient = _shareServiceClient.GetShareClient(_shareName);
                    var rootDir = shareClient.GetRootDirectoryClient();
                    var fileClient = rootDir.GetFileClient(fileName);

                    if (await fileClient.ExistsAsync())
                    {
                        var downloadInfo = await fileClient.DownloadAsync();
                        return downloadInfo.Value.Content;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred while downloading the file: {ex.Message}", ex);
                }
            }

            public async Task DeleteFileAsync(string fileName)
            {
                try
                {
                    var shareClient = _shareServiceClient.GetShareClient(_shareName);
                    var rootDir = shareClient.GetRootDirectoryClient();
                    var fileClient = rootDir.GetFileClient(fileName);

                    await fileClient.DeleteIfExistsAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred while deleting the file: {ex.Message}", ex);
                }
            }

            public async Task<List<string>> ListFilesAsync()
            {
                // Get the ShareClient for the specified share name
                var shareClient = _shareServiceClient.GetShareClient(_shareName);

                // Check if the share exists before proceeding
                if (!await shareClient.ExistsAsync())
                {
                    throw new InvalidOperationException($"The specified share '{_shareName}' does not exist.");
                }

                // Get the root directory client
                var rootDir = shareClient.GetRootDirectoryClient();

                // List files and directories
                var fileList = new List<string>();
                await foreach (var item in rootDir.GetFilesAndDirectoriesAsync())
                {
                    if (!item.IsDirectory)
                    {
                        fileList.Add(item.Name);
                    }
                }

                return fileList;
            }
        }
    }
