using LiveShot.API.Events.Selection;

namespace LiveShot.UI.Controls.Panel
{
    public class BottomDragPanel : DragPanel
    {
        protected override (double, double)? GetPositions(OnSelectionChangeArgs args)
        {
            if (ScreenWidth is null || ScreenHeight is null) return null;

            double left = args.NewLeft + args.NewWidth - ActualWidth;
            double top = args.NewTop + args.NewHeight + Gap;

            return (left, top);
        }

        protected override (double, double)? GetMaxPositions(OnSelectionChangeArgs args)
        {
            if (ScreenWidth is null || ScreenHeight is null) return null;

            var maxLeft = (double) (ScreenWidth - ActualWidth - Gap * 2 - ButtonSize);
            var maxTop = (double) (ScreenHeight - ActualHeight - Gap);

            return (maxLeft, maxTop);
        }
    }
}