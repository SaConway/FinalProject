using FFImageLoading.Forms;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MsorLi.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MsorLi.Services
{
    class BlobService
    {
        const string CONNECTION_STRING = "DefaultEndpointsProtocol=https;AccountName=msorli;AccountKey=kVtnQ844mpRHshb0ZKu7WiNf4n6WS2ZqVa/3Yd/COZrHFhKgoMwqKgBg1PLGuacCfZVZbDHTsBvlN6V04BlmqA==;EndpointSuffix=core.windows.net";
        const string BLOB_URL = "https://msorli.blob.core.windows.net/images/";

        static CloudBlobContainer GetContainer()
        {
            var account = CloudStorageAccount.Parse(CONNECTION_STRING);
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
				byte[] resizedImage = ImageResizer.ResizeImage(imageData, 640, 640); 

                //Insert Image to Blob server
                var imageUrl = await BlobService.UploadFileAsync(new MemoryStream(resizedImage));
                imageUrls.Add(BLOB_URL + imageUrl);
            }

            return imageUrls;
        }

        public static async Task<string> SaveImageInBlob(byte[] byteData)
        {
            byte[] resizedImage = ImageResizer.ResizeImage(byteData, 640, 480);//480
            var imageUrl = await BlobService.UploadFileAsync(new MemoryStream(resizedImage));
            imageUrl = BLOB_URL + imageUrl;

            return imageUrl;
        }


        public static async Task DeleteImage(string image) // Name in the container
        {
            try
            {
                string toBeSearched = BLOB_URL;
                string imageIdToDelete = image.Substring(image.IndexOf(toBeSearched, StringComparison.CurrentCulture) + toBeSearched.Length);
                var container = GetContainer();

                var fileBlob = container.GetBlockBlobReference(imageIdToDelete);
                await fileBlob.DeleteAsync();
            }
            catch(Exception)
            {
                
            }
        }
              
    }
}
