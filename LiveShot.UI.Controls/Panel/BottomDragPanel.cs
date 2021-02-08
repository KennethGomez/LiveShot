using System.Windows.Controls;
using LiveShot.API.Events.Selection;

namespace LiveShot.UI.Controls.Panel
{
    public class BottomDragPanel : DragPanel
    {
        protected override void SetPanelPosition(OnSelectionChangeArgs args)
        {
            if (ScreenWidth is null || ScreenHeight is null) return;

            double left = args.NewLeft + args.NewWidth - ActualWidth;
            double top = args.NewTop + args.NewHeight + Gap;

            var maxLeft = (double) (ScreenWidth - ActualWidth - Gap * 2 - ButtonSize);

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