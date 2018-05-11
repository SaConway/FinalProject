using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using MsorLi.Services;
using MsorLi.Utilities;

namespace MsorLi.Droid
{
    [Activity(Label = "מסור-לי", Theme = "@style/Splash", MainLauncher = false, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Task t1 = Task1();
            Task t2 = Task2();

            Constants.ScreenHeight = (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density);
            Constants.ScreenWidth = (int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density);

            FFImageLoading.Forms.Droid.CachedImageRenderer.Init(true);
            ImageCircle.Forms.Plugin.Droid.ImageCircleRenderer.Init();
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

            await Task.WhenAll(t1, t2);

            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.AddFlags(ActivityFlags.SingleTop);
            StartActivity(intent);
            Finish();

            //StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

        // Launches the startup task
        //protected async override void OnResume()
        //{
        //    base.OnResume();

            
        //}

        // Simulates background work that happens behind the splash screen
        //async void SimulateStartup()
        //{
            
        //}

        async Task Task1()
        {
            await AzureImageService.DefaultManager.GetAllPriorityImages(0,"כל המוצרים", "");
        }

        async Task Task2()
        {
            await CategoryStorage.GetCategories();
        }
    }
}