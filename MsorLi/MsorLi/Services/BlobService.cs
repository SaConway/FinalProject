using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MsorLi.Services
{
    class BlobService
    {
        public BlobService()
        {
            _fullResContainer = _blobClient.GetContainerReference("images");
            //_lowResContainer = _blobClient.GetContainerReference("lowres");
        }

        CloudBlobClient _blobClient = CloudStorageAccount
            .Parse(connectionString)
            .CreateCloudBlobClient();

        const string connectionString = "DefaultEndpointsProtocol=https;AccountName=msorli;AccountKey=kVtnQ844mpRHshb0ZKu7WiNf4n6WS2ZqVa/3Yd/COZrHFhKgoMwqKgBg1PLGuacCfZVZbDHTsBvlN6V04BlmqA==;EndpointSuffix=core.windows.net";

        CloudBlobContainer _fullResContainer;
        //CloudBlobContainer _lowResContainer;

        public async Task<List<Uri>> GetAllBlobUriAsync()
        {
            var contToken = new BlobContinuationToken();
            var allBlobs = await _fullResContainer.ListBlobsSegmentedAsync(contToken).ConfigureAwait(false);

            var uris = allBlobs.Results.Select(b => b.Uri).ToList();

            return uris;
        }

        public async Task UploadImagesAsync(string localPath)
        {
            var uniqueBlobName = Guid.NewGuid().ToString();
            uniqueBlobName += Path.GetExtension(localPath);

            var blobRef = _fullResContainer.GetBlockBlobReference(uniqueBlobName);

            await blobRef.UploadFromFileAsync(localPath).ConfigureAwait(false);
        }

        static CloudBlobContainer GetContainer(string containerType)
        {
            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudBlobClient();
            return client.GetContainerReference(containerType.ToString().ToLower());
        }

        public static async Task<string> UploadFileAsync(string containerType, Stream stream)
        {
            var container = GetContainer(containerType);
            await container.CreateIfNotExistsAsync();

            var name = Guid.NewGuid().ToString();
            var fileBlob = container.GetBlockBlobReference(name);
            await fileBlob.UploadFromStreamAsync(stream);

            return name;
        }
    }
}
