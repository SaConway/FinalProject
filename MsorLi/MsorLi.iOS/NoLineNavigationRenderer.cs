using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using MsorLi.iOS;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(NoLineNavigationRenderer))]
namespace MsorLi.iOS
{
    public class NoLineNavigationRenderer : NavigationRenderer
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // remove lower border and shadow of the navigation bar
            NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
            NavigationBar.ShadowImage = new UIImage();
        }
    }
}