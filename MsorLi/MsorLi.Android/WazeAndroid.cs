using Android.Content;
using MsorLi.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(WazeAndroid))]
namespace MsorLi.Droid
{
    public class WazeAndroid : Utilities.IWaze
    {
        public void Navigate(string location)
        {
            try
            {
                // Launch Waze to look for location:
                Android.Net.Uri uri = Android.Net.Uri.Parse("https://waze.com/ul?q=" + location);
                var intent = new Intent(Intent.ActionView, uri);
#pragma warning disable CS0618 // Type or member is obsolete
                Forms.Context.StartActivity(intent);
#pragma warning restore CS0618 // Type or member is obsolete
            }
            catch (ActivityNotFoundException ex)
            {
                // If Waze is not installed, open it in Google Play:
                Android.Net.Uri uri = Android.Net.Uri.Parse("market://details?id=com.waze");
                var intent = new Intent(Intent.ActionView, uri);
#pragma warning disable CS0618 // Type or member is obsolete
                Forms.Context.StartActivity(intent);
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }
    }
}