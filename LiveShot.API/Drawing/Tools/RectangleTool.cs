using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LiveShot.API.Canvas;
using LiveShot.API.Utils;

namespace LiveShot.API.Drawing.Tools
{
    public class RectangleTool : DrawingTool
    {
        private Rectangle? _rectangle;

        public override CanvasTool Tool => CanvasTool.Rectangle;

        public override void OnMouseLeftButtonDown(MouseButtonEventArgs e, AbstractDrawCanvas canvas)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            var lastPoint = e.GetPosition(canvas);

            LastPoint = lastPoint;

            Rectangle rectangle = new()
            {
                Stroke = canvas.DrawingColor,
                Width = 0,
                Height = 0,
                Fill = Brushes.Transparent,
                StrokeThickness = canvas.DrawingStrokeThickness
            };

            canvas.Children.Add(rectangle);

            _rectangle = rectangle;

            System.Windows.Controls.Canvas.SetLeft(rectangle, lastPoint.X);
            System.Windows.Controls.Canvas.SetTop(rectangle, lastPoint.Y);
        }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e, AbstractDrawCanvas canvas)
        {
            _rectangle = null;
        }

        public override void OnMouseMove(MouseEventArgs e, AbstractDrawCanvas canvas)
        {
            if (LastPoint is not { } lastPoint || _rectangle is null) return;

            var point = e.GetPosition(canvas);

            (double left, double top) = PointUtils.GetCoords(lastPoint, point);

            _rectangle.Width = Math.Abs(lastPoint.X - point.X);
            _rectangle.Height = Math.Abs(lastPoint.Y - point.Y);

            System.Windows.Controls.Canvas.SetLeft(_rectangle, left);
            System.Windows.Controls.Canvas.SetTop(_rectangle, top);
        }
    }
}