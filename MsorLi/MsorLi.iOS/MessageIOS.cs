using MsorLi.iOS;
using GlobalToast;

[assembly: Xamarin.Forms.Dependency(typeof(MessageIOS))]
namespace MsorLi.iOS
{
    public class MessageIOS : MsorLi.Utilities.IMessage
    {
        public void LongAlert(string message)
        {
            Toast.MakeToast(message).Show();
        }
        public void ShortAlert(string message)
        {
            Toast.MakeToast(message);
        }
    }
}
