using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using MsorLi.Services;
using MsorLi.Utilities;

namespace MsorLi.Droid
{
    [Activity(Label = "מסור-לי", Theme = "@style/Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        // Launches the startup task
        protected async override void OnResume()
        {
            base.OnResume();

            Task t1 = Task1();
            Task t2 = Task2();

            Constants.ScreenHeight = (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density);
            Constants.ScreenWidth = (int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density);

            FFImageLoading.Forms.Droid.CachedImageRenderer.Init(true);
            ImageCircle.Forms.Plugin.Droid.ImageCircleRenderer.Init();
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

            await Task.WhenAll(t1, t2);

            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

        // Simulates background work that happens behind the splash screen
        //async void SimulateStartup()
        //{
            
        //}

        async Task Task1()
        {
            await AzureImageService.DefaultManager.GetAllPriorityImages("כל המוצרים", "");
        }

        async Task Task2()
        {
            await CategoryStorage.GetCategories();
        }
    }
}