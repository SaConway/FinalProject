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
        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();
        }

        // Simulates background work that happens behind the splash screen
        async void SimulateStartup()
        {
            Task t1 = Task1();
            Task t2 = Task2();

            await Task.WhenAll(t1, t2);

            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

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