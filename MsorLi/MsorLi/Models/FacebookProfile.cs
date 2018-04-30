using Newtonsoft.Json;

namespace MsorLi.Models
{
    public class FacebookProfile
    {
        public string Name { get; set; }
        public Picture Picture { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("id")]
        public string FacebookId { get; set; }
    }

    public class Picture
    {
        public Data Data { get; set; }
    }
    public class Data
    {
        public bool IsSilhouette { get; set; }
        public string Url { get; set; }
    }





}
