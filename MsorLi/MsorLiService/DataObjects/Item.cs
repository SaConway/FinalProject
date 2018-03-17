using Microsoft.Azure.Mobile.Server;

namespace MsorLi.DataObjects
{
    public class Item : EntityData
    {
        public string Title { get; set; }
        public int NumOfImages { get; set; }
        public string ImageUrl_1 { get; set; }
        public string ImageUrl_2 { get; set; }
        public string ImageUrl_3 { get; set; }
        public string ImageUrl_4 { get; set; }
        public string Description { get; set; }
        public string Condition { get; set; }
        public string Location { get; set; }
        public int ViewCounter { get; set; }
        public string ContactName { get; set; }
        public string ContactNumber { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
    }
}