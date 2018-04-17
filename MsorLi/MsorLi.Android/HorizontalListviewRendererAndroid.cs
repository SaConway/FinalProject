using System.ComponentModel;
using MsorLi.Utilities;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HorizontalListview), typeof(MsorLi.Droid.Renderers.HorizontalListviewRendererAndroid))]

namespace MsorLi.Droid.Renderers
{
#pragma warning disable CS0618 // Type or member is obsolete
    public class HorizontalListviewRendererAndroid : ScrollViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            var element = e.NewElement as HorizontalListview;
            element?.Render();

            if (e.OldElement != null)
                e.OldElement.PropertyChanged -= OnElementPropertyChanged;

            e.NewElement.PropertyChanged += OnElementPropertyChanged;

        }

        protected void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.HorizontalScrollBarEnabled = false;
            this.VerticalScrollBarEnabled = false;
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}