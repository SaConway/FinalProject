using System;
using MsorLi.iOS.Renderers;
using UIKit;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(ViewCell), typeof(ViewCellRenderer))]

namespace MsorLi.iOS.Renderers
{
    //This class remove Gray background color from IOS Device "listview" when item selected
    public class ViewCellRenderer : Xamarin.Forms.Platform.iOS.ViewCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            reusableCell = base.GetCell(item, reusableCell, tv);

            //First working solution (Works alone without second one).
            reusableCell.SelectionStyle = UITableViewCellSelectionStyle.None;

            return reusableCell;
        }
    }
}