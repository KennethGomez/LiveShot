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

        protected abstract void SetPanelPosition(OnSelectionChangeArgs args);
    }
}