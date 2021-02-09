using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LiveShot.API.Drawing.Tools
{
    public class RectangleTool : DrawingTool
    {
        private Rectangle? _rectangle;
        
        public override CanvasTool Tool => CanvasTool.Rectangle;

        public override void OnMouseLeftButtonDown(MouseButtonEventArgs e, System.Windows.Controls.Canvas canvas)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            LastPoint = e.GetPosition(canvas);
            
            var lastPoint = LastPoint.Value;

            Rectangle rectangle = new()
            {
                Stroke = Color,
                Width = 0,
                Height = 0,
                Fill = Brushes.Transparent,
            };

            canvas.Children.Add(rectangle);

            _rectangle = rectangle;

            System.Windows.Controls.Canvas.SetLeft(rectangle, lastPoint.X);
            System.Windows.Controls.Canvas.SetTop(rectangle, lastPoint.Y);
        }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e, System.Windows.Controls.Canvas canvas)
        {
            _rectangle = null;
        }

        public override void OnMouseMove(MouseEventArgs e, System.Windows.Controls.Canvas canvas)
        {
            if (LastPoint is null || _rectangle is null) return;

            var lastPoint = LastPoint.Value;
            var point = e.GetPosition(canvas);

            double xDiff = lastPoint.X - point.X;
            double yDiff = lastPoint.Y - point.Y;

            bool growingX = xDiff > 0;
            bool growingY = yDiff > 0;

            double rectLeft = growingX ? point.X : lastPoint.X;
            double rectTop = growingY ? point.Y : lastPoint.Y;
            
            _rectangle.Width = Math.Abs(xDiff);
            _rectangle.Height = Math.Abs(yDiff);
            
            System.Windows.Controls.Canvas.SetLeft(_rectangle, rectLeft);
            System.Windows.Controls.Canvas.SetTop(_rectangle, rectTop);
        }
    }
}