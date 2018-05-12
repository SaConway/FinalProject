using Microsoft.Azure.Mobile.Server;

namespace MsorLiService.DataObjects
{
    public class Item : EntityData
    {
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public int NumOfImages { get; set; }
        public string Description { get; set; }
        public string Condition { get; set; }
        public string Location { get; set; }
        public int ViewCounter { get; set; }
        public string ContactName { get; set; }
        public string ContactNumber { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
    }
}