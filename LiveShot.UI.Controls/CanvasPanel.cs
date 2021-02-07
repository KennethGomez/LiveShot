using System.Windows.Controls;
using LiveShot.API;
using LiveShot.API.Events;
using LiveShot.API.Events.Selection;

namespace LiveShot.UI.Controls
{
    public class CanvasPanel : StackPanel
    {
        private const int Gap = 10;
        
        private IEventPipeline? _events;
        private double? _screenWidth;
        private double? _screenHeight;
        
        public CanvasPanel()
        {
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
            var args = e.GetArgs<OnSelectionChangeArgs>();

            if (Orientation == Orientation.Vertical)
            {
                SetRightPanelPosition(args);
            }
            else
            {
                SetLeftPanelPosition(args);
            }
        }

        private void SetLeftPanelPosition(OnSelectionChangeArgs args)
        {
            if (_screenWidth is null || _screenHeight is null) return;

            double left = args.NewLeft + args.NewWidth - ActualWidth;
            double top = args.NewTop + args.NewHeight + Gap;
            
            Canvas.SetLeft(this, left);
            Canvas.SetTop(this, top);
        }

        private void SetRightPanelPosition(OnSelectionChangeArgs args)
        {
            if (_screenWidth is null || _screenHeight is null) return;

            double left = args.NewWidth + args.NewLeft + Gap;
            double top = args.NewTop;

            var maxLeft = (double) (_screenWidth - ActualWidth - Gap);

            if (left > maxLeft)
            {
                left = args.NewWidth + args.NewLeft - Gap - ActualWidth;
                top += Gap;

                if (left > maxLeft)
                {
                    left = maxLeft;
                }
            }
            
            var maxTop = (double) (_screenHeight - ActualHeight - Gap);

            if (top > maxTop)
            {
                top = maxTop;
            }
            
            Canvas.SetLeft(this, left);
            Canvas.SetTop(this, top);
        }
    }
}