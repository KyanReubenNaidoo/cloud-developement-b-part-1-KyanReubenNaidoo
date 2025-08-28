using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace POECLDV6212.Services
{
    public class Blob_Service
    {
        private readonly BlobServiceClient _blob;
        private readonly string _container = "images";

        public Blob_Service(string connectionString)
        {
            _blob = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadsAsync(Stream fileStream, string fileName)
        {
            var containerClient = _blob.GetBlobContainerClient(_container);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream);
            return blobClient.Uri.ToString();
        }

        public async Task DeleteBlobAsync(string blobUri)
        {
            Uri uri = new Uri(blobUri);
            string blobName = uri.Segments[^1];
            var containerClient = _blob.GetBlobContainerClient(_container);
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }


    }
}