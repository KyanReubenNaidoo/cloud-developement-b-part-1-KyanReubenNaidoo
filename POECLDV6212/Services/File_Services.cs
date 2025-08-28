using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

using POECLDV6212.Models;

namespace POECLDV6212.Services
{
    public class File_Service
    {
        private readonly string _connectionString;
        private readonly string _file;

        public File_Service(string connectionString, string file)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _file = file ?? throw new ArgumentNullException(nameof(file));
        }

        public async Task UploadFileAsync(string directoryName, string fileName, Stream stream)
        {
            try
            {
                var serviceClient = new ShareServiceClient(_connectionString);
                var shareClient = serviceClient.GetShareClient(_file);

                var directoryClient = shareClient.GetDirectoryClient(directoryName);
                await directoryClient.CreateIfNotExistsAsync();

                var fileClient = directoryClient.GetFileClient(fileName);

                await fileClient.CreateAsync(stream.Length);
                await fileClient.UploadRangeAsync(new HttpRange(0, stream.Length), stream);
            }
            catch (Exception ex)
            {
                throw new Exception("Error uploading file : " + ex.Message, ex);
            }
        }

        public async Task<Stream> DownloadFileAsync(string directoryName, string fileName)
        {
            try
            {
                var serviceClient = new ShareServiceClient(_connectionString);
                var shareClient = serviceClient.GetShareClient(_file);
                var directoryClient = shareClient.GetDirectoryClient(directoryName);
                var fileClient = directoryClient.GetFileClient(fileName);
                var download = await fileClient.DownloadAsync();
                return download.Value.Content;
            }
            catch (Exception ex)
            {
                throw new Exception("Error downloading file : " + ex.Message, ex);
            }
        }

        public async Task<List<FileModel>> ListFilesAsync(string directoryName)
        {
            var fileModels = new List<FileModel>();
            try
            {
                var serviceClient = new ShareServiceClient(_connectionString);
                var shareClient = serviceClient.GetShareClient(_file);
                var directoryClient = shareClient.GetDirectoryClient(directoryName);

                await foreach (ShareFileItem item in directoryClient.GetFilesAndDirectoriesAsync())
                {
                    if (!item.IsDirectory)
                    {
                        var fileClient = directoryClient.GetFileClient(item.Name);
                        var properties = await fileClient.GetPropertiesAsync();
                        fileModels.Add(new FileModel
                        {
                            Name = item.Name,
                            Size = properties.Value.ContentLength,
                            LastModified = properties.Value.LastModified
                        });

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error listing files: " + ex.Message, ex);
            }
            return fileModels;
        }

        public async Task DeleteFileAsync(string directoryName, string fileName)
        {
            try
            {
                var serviceClient = new ShareServiceClient(_connectionString);
                var shareClient = serviceClient.GetShareClient(_file);
                var directoryClient = shareClient.GetDirectoryClient(directoryName);
                var fileClient = directoryClient.GetFileClient(fileName);

                // Delete file if it exists
                await fileClient.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting file: " + ex.Message, ex);
            }
        }

    }
}