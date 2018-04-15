using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MsorLi.Services;

namespace MsorLi.Utilities
{
    public static class ImageUpload
    {
        //Convert from Stream to array of bytes
        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

    }
}
