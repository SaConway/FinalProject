using MsorLi.iOS.Renderers.Button;
using UIKit;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(Button), typeof(iOSButtonRenderer))]
namespace MsorLi.iOS.Renderers.Button
{
    //This Class add padding to iOS Buttons
    public class iOSButtonRenderer : Xamarin.Forms.Platform.iOS.ButtonRenderer
    {
        public iOSButtonRenderer() { }

        protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);

            Control.ContentEdgeInsets = new UIEdgeInsets(
            Control.ContentEdgeInsets.Top,
            Control.ContentEdgeInsets.Left + 10,
            Control.ContentEdgeInsets.Bottom,
            Control.ContentEdgeInsets.Right + 10);

            //For MultiLine Text
            Control.TitleLabel.LineBreakMode = UILineBreakMode.WordWrap;
            Control.TitleLabel.TextAlignment = UITextAlignment.Center;
        }
    }
}
