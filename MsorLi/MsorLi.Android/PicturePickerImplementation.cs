using MsorLi.Droid;
using System.Threading.Tasks;
using Android.Content;
using Xamarin.Forms;
using System.IO;

[assembly: Dependency(typeof(PicturePickerImplementation))]

namespace MsorLi.Droid
{
    public class PicturePickerImplementation : Services.IPicturePicker
    {
        public Task<Stream> GetImageStreamAsync()
        {
            // Define the Intent for getting images
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);

            // Get the MainActivity instance
#pragma warning disable CS0618 // Type or member is obsolete
            MainActivity activity = Forms.Context as MainActivity;
#pragma warning restore CS0618 // Type or member is obsolete

            // Start the picture-picker activity (resumes in MainActivity.cs)
            activity.StartActivityForResult(
                Intent.CreateChooser(intent, "Select Picture"),
                MainActivity.PickImageId);

            // Save the TaskCompletionSource object as a MainActivity property
            activity.PickImageTaskCompletionSource = new TaskCompletionSource<Stream>();

            // Return Task object
            return activity.PickImageTaskCompletionSource.Task;
        }
    }
}
