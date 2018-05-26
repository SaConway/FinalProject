using Foundation;
using SuaveControls.FloatingActionButton.iOS.Renderers;
using UIKit;
using Xamarin.Forms;


namespace MsorLi.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {

            global::Xamarin.Forms.Forms.Init();

            Rg.Plugins.Popup.Popup.Init();

            //init for the circle image plugin
            ImageCircle.Forms.Plugin.iOS.ImageCircleRenderer.Init();
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

            Utilities.Constants.ScreenHeight = (int)UIScreen.MainScreen.Bounds.Height;
            Utilities.Constants.ScreenWidth = (int)UIScreen.MainScreen.Bounds.Width;

            FloatingActionButtonRenderer.InitRenderer();

            var statusBar = UIApplication.SharedApplication.ValueForKey(new NSString("statusBar")) as UIView;
            if (statusBar.RespondsToSelector(new ObjCRuntime.Selector("setBackgroundColor:")))
            {
                statusBar.BackgroundColor = UIColor.FromRGB(25,164,180);
                statusBar.TintColor = UIColor.White;
            }

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }

}
