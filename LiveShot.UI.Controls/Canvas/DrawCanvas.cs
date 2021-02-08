using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LiveShot.UI.Controls.Canvas
{
    public class DrawCanvas : System.Windows.Controls.Canvas
    {
        public static CanvasAction Action = CanvasAction.Default;

        private Point _currentPoint;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && Action != CanvasAction.Default)
                _currentPoint = e.GetPosition(this);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || Action == CanvasAction.Default) return;

            Line line = new()
            {
                Stroke = Brushes.Red,
                X1 = _currentPoint.X,
                Y1 = _currentPoint.Y,
                X2 = e.GetPosition(this).X,
                Y2 = e.GetPosition(this).Y
            };

            _currentPoint = e.GetPosition(this);

            Children.Add(line);
        }
    }
}