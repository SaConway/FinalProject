using Microsoft.Azure.Mobile.Server;

namespace MsorLiService.DataObjects
{
    public class SavedItem : EntityData
    {
        public string UserId { get; set; }
        public string ItemId { get; set; }
    }
}