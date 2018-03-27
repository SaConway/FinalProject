using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;
using MsorLi.Droid;
using MsorLi.Views;
using MsorLi.Utilities;
using Android.Support.Design.Widget;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(MainPage), typeof(MyTabsRenderer))]
namespace MsorLi.Droid
{
    public class MyTabsRenderer : TabbedPageRenderer, TabLayout.IOnTabSelectedListener
    {
        private MainPage _page;

        public MyTabsRenderer()
        {
        }

        //protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
        //{
        //    base.OnElementChanged(e);

        //    if (e.NewElement != null)
        //    {
        //        _page = (MainPage)e.NewElement;
        //    }
        //    else
        //    {
        //        _page = (MainPage)e.OldElement;
        //    }
        //}

        //async void TabLayout.IOnTabSelectedListener.OnTabSelected(TabLayout.Tab tab)
        //{
        //    if (tab.Position == 2)  // Add Item Page
        //    {
        //        if (Settings._GeneralSettings != "True")
        //        {
        //            _page.CurrentPage = _page.Children[0];
        //            await _page.CurrentPage.Navigation.PushAsync(new LoginPage());
        //        }
        //    }
        //}
    }
}