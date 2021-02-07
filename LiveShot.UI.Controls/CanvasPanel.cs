using System.Windows.Controls;
using LiveShot.API;
using LiveShot.API.Events;
using LiveShot.API.Events.Selection;

namespace LiveShot.UI.Controls
{
    public class CanvasPanel : StackPanel
    {
        private IEventPipeline? _events;
        private double? _screenWidth;
        private double? _screenHeight;
        
        public CanvasPanel()
        {
            Orientation = Orientation.Vertical;

            SetValue(ZIndexProperty, 1);
        }

        public void With(IEventPipeline events, double width, double height)
        {
            _events = events;
            _screenWidth = width;
            _screenHeight = height;
            
            _events.Subscribe<OnSelectionChange>(OnSelectionChange);
        }

        private void OnSelectionChange(Event e)
        {
            if (_screenWidth is null || _screenHeight is null) return;
            
            var args = e.GetArgs<OnSelectionChangeArgs>();

            const double gap = 10;

            double left = args.NewWidth + args.NewLeft + gap;
            double top = args.NewTop;

            var maxLeft = (double) (_screenWidth - ActualWidth - gap);

            if (left > maxLeft)
            {
                left = args.NewWidth + args.NewLeft - gap - ActualWidth;
                top += gap;

                if (left > maxLeft)
                {
                    left = maxLeft;
                }
            }
            
            var maxTop = (double) (_screenHeight - ActualHeight - gap);

            if (top > maxTop)
            {
                top = maxTop;
            }
            
            Canvas.SetLeft(this, left);
            Canvas.SetTop(this, top);
        }
    }
}