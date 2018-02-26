using System;
using System.IO;
using System.Threading.Tasks;

namespace MsorLi.Services
{
    public interface IPicturePicker
    {
        Task<Stream> GetImageStreamAsync();
    }
}