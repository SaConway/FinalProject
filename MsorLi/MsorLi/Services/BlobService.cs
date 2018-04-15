using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MsorLi.Utilities;
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

        public static async Task<List<string>> SaveImagesInDB(List<byte[]> byteData)
        {
            List<string> imageUrls = new List<string>();

            foreach (var imageData in byteData)
            {
                byte[] resizedImage = ImageResizer.ResizeImage(imageData, 400, 400);

                //Insert Image to Blob server
                var imageUrl = await BlobService.UploadFileAsync(new MemoryStream(resizedImage));
                imageUrls.Add("https://msorli.blob.core.windows.net/images/" + imageUrl);
            }

            return imageUrls;
        }
    }
}
