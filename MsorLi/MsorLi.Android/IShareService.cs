using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using MsorLi.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(IShareService))]
namespace MsorLi.Droid
{
    public class IShareService : Activity, Utilities.IShare
    {
        public async void Share(string subject, string message, ImageSource image)
        {
            try
            {
                var intent = new Intent(Intent.ActionSend);
                intent.PutExtra(Intent.ExtraSubject, subject);
                intent.PutExtra(Intent.ExtraText, message);
                intent.SetType("image/png");

                var handler = new ImageLoaderSourceHandler();
                var bitmap = await handler.LoadImageAsync(image, this);

                var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads
                    + Java.IO.File.Separator + "logo.png");

                using (var os = new System.IO.FileStream(path.AbsolutePath, System.IO.FileMode.Create))
                {
                    bitmap.Compress(Bitmap.CompressFormat.Png, 100, os);
                }

                intent.PutExtra(Intent.ExtraStream, Android.Net.Uri.FromFile(path));
                Forms.Context.StartActivity(Intent.CreateChooser(intent, "Share Image"));
            }
            catch (System.Exception)
            {
                Toast.MakeText(Forms.Context as MainActivity, "יש לוודא הרשאות לפני שימוש", ToastLength.Long).Show();
            }
        }
    }
}