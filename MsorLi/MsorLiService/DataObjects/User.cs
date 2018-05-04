using Microsoft.Azure.Mobile.Server;

namespace MsorLiService.DataObjects
{
    public class User : EntityData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Permission { get; set; }
        public string ImgUrl { get; set; }
        public int NumOfItems { get; set; }
        public int NumOfItemsUserLike { get; set; }
        public string FacebookId { get; set; }
    }
}