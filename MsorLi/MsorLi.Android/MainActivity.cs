using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using MsorLi.Utilities;
using Plugin.Toasts;
using Xamarin.Forms;


namespace MsorLi.Droid
{
    [Activity(Label = "מסור-לי", Icon = "@drawable/icon", Theme = "@style/Splash",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                TabLayoutResource = Resource.Layout.Tabbar;
                ToolbarResource = Resource.Layout.Toolbar;

                Utilities.Constants.ScreenHeight = (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density);
                Utilities.Constants.ScreenWidth = (int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density);

                FFImageLoading.Forms.Droid.CachedImageRenderer.Init(true);
                ImageCircle.Forms.Plugin.Droid.ImageCircleRenderer.Init();
                Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

                global::Xamarin.Forms.Forms.Init(this, bundle);

                DependencyService.Register<ToastNotification>(); // Register your dependency
                ToastNotification.Init(this);

                base.SetTheme(Resource.Style.MainTheme);
                base.OnCreate(bundle);
<<<<<<< HEAD
=======
            }
            catch (System.Exception)
            {

<<<<<<< Updated upstream
            }
        }

        protected async override void OnResume()
        {
            try
            {
                base.OnResume();

                var categories = await CategoryStorage.GetCategories();

>>>>>>> cb731558bb7723d882f86325a1c583aef2fae7c2
                LoadApplication(new App());

            }
            catch (System.Exception)
            {

            }
        }

=======
            base.SetTheme(Resource.Style.MainTheme);
            base.OnCreate(bundle);

            Rg.Plugins.Popup.Popup.Init(this, bundle);
            LoadApplication(new App());
        }


        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }
        
>>>>>>> Stashed changes
        //--------------------------------------------
        // PICTURE HANDLE

        // Field, property, and method for Picture Picker
        public static readonly int PickImageId = 1000;

        public TaskCompletionSource<Stream> PickImageTaskCompletionSource { set; get; }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            base.OnActivityResult(requestCode, resultCode, intent);

            if (requestCode == PickImageId)
            {
                if ((resultCode == Result.Ok) && (intent != null))
                {
                    Android.Net.Uri uri = intent.Data;
                    Stream stream = ContentResolver.OpenInputStream(uri);

                    // Set the Stream as the completion of the Task
                    PickImageTaskCompletionSource.SetResult(stream);
                }
                else
                {
                    PickImageTaskCompletionSource.SetResult(null);
                }
            }
        }
    }
}