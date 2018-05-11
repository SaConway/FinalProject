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
                // Launch Waze to look for location:
                UIApplication.SharedApplication.OpenUrl(new NSUrl("https://waze.com/ul?q="));
            }
            catch (Exception)
            {
                // If Waze is not installed, open it in Itunes:
                UIApplication.SharedApplication.OpenUrl(new NSUrl("http://itunes.apple.com/us/app/id323229106"));
            }
        }
    }
}