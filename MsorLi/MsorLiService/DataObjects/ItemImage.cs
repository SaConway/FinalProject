﻿using Microsoft.Azure.Mobile.Server;

namespace MsorLiService.DataObjects
{
    public class ItemImage : EntityData
    {
        public string ItemId { get; set; }
        public string Url { get; set; }
        public bool IsPriorityImage { get; set; }
        public string UserId { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Erea { get; set; }
        public string Condition { get; set; }
    }
}