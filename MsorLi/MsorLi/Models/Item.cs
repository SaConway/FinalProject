using System;
using System.Collections.Generic;
using System.Text;

namespace MsorLi.Models
{
    public class Item
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }

        [Microsoft.WindowsAzure.MobileServices.Version]
        public string AzureVersion { get; set; }
    }
}
