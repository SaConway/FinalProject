using Microsoft.Azure.Mobile.Server;

namespace MsorLiService.DataObjects
{
    public class Location : EntityData
    {
        public string Name { get; set; }
        public int Order { get; set; }
    }
}