using System.ComponentModel;
using MsorLi.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ScrollView), typeof(ScrollViewEXRenderer))]
namespace MsorLi.Droid
{
#pragma warning disable CS0618 // Type or member is obsolete
    public class ScrollViewEXRenderer : ScrollViewRenderer

#pragma warning restore CS0618 // Type or member is obsolete
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            //if (e.OldElement == null || this.Element == null)
                //return;

            if (e.OldElement != null)
                e.OldElement.PropertyChanged -= OnElementPropertyChanged;

            e.NewElement.PropertyChanged += OnElementPropertyChanged;

        }

        protected void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ChildCount > 0)
            {
                GetChildAt(0).HorizontalScrollBarEnabled = false;
                GetChildAt(0).VerticalScrollBarEnabled = false;
            }
        }
    }
}