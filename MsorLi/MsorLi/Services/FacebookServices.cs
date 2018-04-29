using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MsorLi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace MsorLi.Services
{


    public static class FacebookServices
    {
        public static async Task<FacebookProfile> GetFacebookProfileAsync(string accessToken)
        {
            var requestUrl =
                "https://graph.facebook.com/v2.7/me/?fields=name,picture.type(large),address,email,first_name,last_name&access_token="
            + accessToken;

            var httpClient = new HttpClient();
            string userJson = await httpClient.GetStringAsync(requestUrl);
            return JsonConvert.DeserializeObject<FacebookProfile>(userJson); 
        }

        public static string ExtractAccessTokenFromUrl(string url)
        {
            if (url.Contains("access_token") && url.Contains("&expires_in="))
            {
                var at = url.Replace("https://www.facebook.com/connect/login_success.html#access_token=", "");

                var accessToken = at.Remove(at.IndexOf("&expires_in=", StringComparison.CurrentCulture));

                return accessToken;
            }

            return string.Empty;
        }
    }
}
