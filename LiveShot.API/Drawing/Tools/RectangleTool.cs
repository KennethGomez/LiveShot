using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LiveShot.API.Utils;

namespace LiveShot.API.Drawing.Tools
{
    public class RectangleTool : DrawingTool
    {
        private Rectangle? _rectangle;

        private readonly ILiveShotService _liveShotService;

        public RectangleTool(ILiveShotService liveShotService)
        {
            _liveShotService = liveShotService;
        }

        public override CanvasTool Tool => CanvasTool.Rectangle;

        public override UIElement? OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || _liveShotService.DrawCanvas is not { } canvas) return null;

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
            
            return null;
        }

        public override UIElement? OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            var tmp = _rectangle;

            _rectangle = null;

            return tmp;
        }

        public override UIElement? OnMouseMove(MouseEventArgs e)
        {
            if (LastPoint is not { } lastPoint ||
                _rectangle is null ||
                _liveShotService.DrawCanvas is not { } canvas
            ) return null;

            var point = e.GetPosition(canvas);

            (double left, double top) = PointUtils.GetCoords(lastPoint, point);

            double w = Math.Abs(lastPoint.X - point.X);
            double h = Math.Abs(lastPoint.Y - point.Y);

            _rectangle.Width = w;
            _rectangle.Height = h;

            System.Windows.Controls.Canvas.SetLeft(_rectangle, left);
            System.Windows.Controls.Canvas.SetTop(_rectangle, top);

            return null;
        }

        public override void UpdateThickness(double thickness)
        {
            if (_rectangle is not null)
                _rectangle.StrokeThickness = thickness;
        }
    }
}