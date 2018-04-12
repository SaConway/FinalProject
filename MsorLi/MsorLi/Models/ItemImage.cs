

using System;

namespace MsorLi.Models
{
    public class ItemImage
    {
        public string Id { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public string ItemId { get; set; }
        public string Url { get; set; }
        public bool IsPriorityImage { get; set; }
        public string UserId { get; set; }
    }
}
