using LiveShot.API.Events.Selection;

namespace LiveShot.UI.Controls.Panel
{
    public class RightDragPanel : DragPanel
    {
        protected override (double, double)? GetPositions(OnSelectionChangeArgs args)
        {
            if (ScreenWidth is null || ScreenHeight is null) return null;

            double left = args.NewWidth + args.NewLeft + Gap;
            double top = args.NewTop;

            return (left, top);
        }

        protected override (double, double)? GetMaxPositions(OnSelectionChangeArgs args)
        {
            if (ScreenWidth is null || ScreenHeight is null) return null;
            
            var maxLeft = (double) (ScreenWidth - ActualWidth - Gap);
            var maxTop = (double) (ScreenHeight - ActualHeight - Gap);

            return (maxLeft, maxTop);
        }
    }
}