using System;
using UIKit;
using MsorLi.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using MsorLi;
using System.ComponentModel;
using MsorLi.Utilities;

[assembly: ExportRenderer(typeof(HorizontalListview), typeof(MsorLi.iOS.Renderers.HorizontalListviewRendererIos))]

namespace MsorLi.iOS.Renderers
{
    public class HorizontalListviewRendererIos : ScrollViewRenderer
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
            this.ShowsHorizontalScrollIndicator = false;
            this.ShowsVerticalScrollIndicator = false;
            this.AlwaysBounceHorizontal = false;
            this.AlwaysBounceVertical = false;
            this.Bounces = false;

        }
    }
}