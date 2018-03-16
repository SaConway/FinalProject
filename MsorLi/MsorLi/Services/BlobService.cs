using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsorLi.Services
{
    class BlobService
    {
        const string connectionString = "DefaultEndpointsProtocol=https;AccountName=msorli;AccountKey=kVtnQ844mpRHshb0ZKu7WiNf4n6WS2ZqVa/3Yd/COZrHFhKgoMwqKgBg1PLGuacCfZVZbDHTsBvlN6V04BlmqA==;EndpointSuffix=core.windows.net";

        static CloudBlobContainer GetContainer()
        {
            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudBlobClient();
            return client.GetContainerReference("images");
        }

        public static async Task<string> UploadFileAsync(Stream stream)
        {
            var container = GetContainer();
            await container.CreateIfNotExistsAsync();

            var name = Guid.NewGuid().ToString();
            var fileBlob = container.GetBlockBlobReference(name);
            await fileBlob.UploadFromStreamAsync(stream);

            return name;
        }
    }
}
