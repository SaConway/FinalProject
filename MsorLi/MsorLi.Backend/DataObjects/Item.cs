using Microsoft.Azure.Mobile.Server;

namespace MsorLi.DataObjects
{
    public class Item : EntityData
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
    }
}