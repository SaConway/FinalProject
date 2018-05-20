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
#pragma warning disable CS0618 // Type or member is obsolete
            Toast.MakeText(Forms.Context as MainActivity, message, ToastLength.Long).Show();
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public void ShortAlert(string message)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            Toast.MakeText(Forms.Context as MainActivity, message, ToastLength.Short).Show();
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}