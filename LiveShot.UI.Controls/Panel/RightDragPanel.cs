using System.Windows.Controls;
using LiveShot.API.Events.Selection;

namespace LiveShot.UI.Controls.Panel
{
    public class RightDragPanel : DragPanel
    {
        protected override void SetPanelPosition(OnSelectionChangeArgs args)
        {
            if (ScreenWidth is null || ScreenHeight is null) return;

            double left = args.NewWidth + args.NewLeft + Gap;
            double top = args.NewTop;

            var maxLeft = (double) (ScreenWidth - ActualWidth - Gap);

            if (left > maxLeft)
            {
                left = maxLeft;
            }

            var maxTop = (double) (ScreenHeight - ActualHeight - Gap);

            if (top > maxTop)
            {
                top = maxTop;
            }

            Canvas.SetLeft(this, left);
            Canvas.SetTop(this, top);
        }
    }
}