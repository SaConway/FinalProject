using Android.Widget;
using MsorLi.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(MessageAndroid))]
namespace MsorLi.Droid
{
    public class MessageAndroid : Utilities.IMessage
    {
        public void LongAlert(string message)
        {
            Toast.MakeText(Forms.Context as MainActivity, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Forms.Context as MainActivity, message, ToastLength.Short).Show();
        }
    }
}