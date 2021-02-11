using System.Windows.Controls;
using System.Windows.Input;
using LiveShot.API;
using LiveShot.API.Events;
using LiveShot.API.Events.Selection;

namespace LiveShot.UI.Controls.Panel
{
    public abstract class DragPanel : StackPanel
    {
        protected const int Gap = 10;
        protected const int ButtonSize = 36;

        private IEventPipeline? _events;
        protected double? ScreenHeight;

        protected double? ScreenWidth;

        protected DragPanel()
        {
            SetValue(ZIndexProperty, 2);

            ForceCursor = true;
            Cursor = Cursors.Arrow;
        }

        public void With(IEventPipeline events, double width, double height)
        {
            _events = events;
            _events.Subscribe<OnSelectionChange>(OnSelectionChange);

            ScreenWidth = width;
            ScreenHeight = height;
        }

        private void OnSelectionChange(Event e)
        {
            var args = e.GetArgs<OnSelectionChangeArgs>();

            SetPanelPosition(args);
        }

        private void SetPanelPosition(OnSelectionChangeArgs args)
        {
            var positions = GetPositions(args);
            var maxPositions = GetMaxPositions(args);

            if (positions is not { } coords || maxPositions is not { } maxCoords) return;
            
            (double left, double top) = coords;
            (double maxLeft, double maxTop) = maxCoords;

            if (left > maxLeft)
            {
                left = maxLeft;
            }

            if (top > maxTop)
            {
                top = maxTop;
            }
            
            System.Windows.Controls.Canvas.SetLeft(this, left);
            System.Windows.Controls.Canvas.SetTop(this, top);
        }

        protected abstract (double, double)? GetPositions(OnSelectionChangeArgs args);
        protected abstract (double, double)? GetMaxPositions(OnSelectionChangeArgs args);
    }
}