using System.Windows.Controls;
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
            SetValue(ZIndexProperty, 1);
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

            if (!positions.HasValue || !maxPositions.HasValue) return;
            
            (double left, double top) = positions.Value;
            (double maxLeft, double maxTop) = maxPositions.Value;

            if (left > maxLeft)
            {
                left = maxLeft;
            }

            if (top > maxTop)
            {
                top = maxTop;
            }
            
            Canvas.SetLeft(this, left);
            Canvas.SetTop(this, top);
        }

        protected abstract (double, double)? GetPositions(OnSelectionChangeArgs args);
        protected abstract (double, double)? GetMaxPositions(OnSelectionChangeArgs args);
    }
}