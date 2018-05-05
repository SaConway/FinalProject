using Xamarin.Forms;
using MsorLi.iOS;
using UIKit;
using Foundation;
using System;
using System.Web;

[assembly: Dependency(typeof(WazeIOS))]
namespace MsorLi.iOS
{
    class WazeIOS : Utilities.IWaze
    {
        public void Navigate(string location)
        {
            try
            {
                //HttpUtility x;
                string url_encoded_location = System.Net.WebUtility.UrlEncode("ירושלים");
                // Launch Waze to look for location:
                UIApplication.SharedApplication.OpenUrl(new NSUrl("https://waze.com/ul?q="));

              //  var uri = new Uri("https://waze.com/ul?q=ירושלים");
              //  var nsurl = new NSUrl(uri.GetComponents(UriComponents.HttpRequestUrl, UriFormat.UriEscaped));
              //// UIApplication.SharedApplication.OpenUrl(nsurl);
                //var webView = new WebView
                //{
                //    Source = "https://waze.com/ul?q=ירושלים",
                //    HeightRequest = 1
                //};


            }
            catch (Exception)
            {
                // If Waze is not installed, open it in Itunes:
                UIApplication.SharedApplication.OpenUrl(new NSUrl("http://itunes.apple.com/us/app/id323229106"));
            }


   
        }
    }
}