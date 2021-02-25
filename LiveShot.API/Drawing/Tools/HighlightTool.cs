using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LiveShot.API.Drawing.Tools
{
    public class HighlightTool : DrawingTool
    {
        private Polyline? _polyline;

        private readonly ILiveShotService _liveShotService;

        public HighlightTool(ILiveShotService liveShotService)
        {
            _liveShotService = liveShotService;
        }

        public static readonly Brush Color = Brushes.Yellow;
        public const double Opacity = 0.5;

        public override CanvasTool Tool => CanvasTool.Highlight;

        public override UIElement? OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.ButtonState != MouseButtonState.Pressed || _liveShotService.DrawCanvas is not { } canvas)
                return null;

            var point = e.GetPosition(canvas);

            LastPoint = point;

            _polyline = new Polyline
            {
                StrokeThickness = GetStrokeThickness(canvas.DrawingStrokeThickness),
                Stroke = Color,
                Opacity = Opacity,
                StrokeLineJoin = PenLineJoin.Round,
                Points = new PointCollection
                {
                    point
                }
            };

            canvas.Children.Add(_polyline);

            return null;
        }

        public override UIElement? OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            var tmp = _polyline;

            _polyline = null;

            return tmp;
        }

        public override UIElement? OnMouseMove(MouseEventArgs e)
        {
            if (_polyline is null || _liveShotService.DrawCanvas is not { } canvas)
                return null;

            var point = e.GetPosition(canvas);

            LastPoint = point;

            _polyline.Points.Add(point);

            return null;
        }

        public override void UpdateThickness(double thickness)
        {
            if (_polyline is null) return;

            _polyline.StrokeThickness = GetStrokeThickness(thickness);
        }

        private static double GetStrokeThickness(double thickness)
        {
            return thickness * 1.5 + 4;
        }
    }
}