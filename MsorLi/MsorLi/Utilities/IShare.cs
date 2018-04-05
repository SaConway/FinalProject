using Xamarin.Forms;

namespace MsorLi.Utilities
{
    public interface IShare
    {
        void Share(string subject, string message, ImageSource image);
    }
}