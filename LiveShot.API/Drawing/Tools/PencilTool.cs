using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LiveShot.API.Drawing.Tools
{
    public class PencilTool : DrawingTool
    {
        private Polyline? _polyline;

        private readonly ILiveShotService _liveShotService;

        public PencilTool(ILiveShotService liveShotService)
        {
            _liveShotService = liveShotService;
        }

        public override CanvasTool Tool => CanvasTool.Pencil;

        public override UIElement? OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || _liveShotService.DrawCanvas is not { } canvas) return null;

            var point = e.GetPosition(canvas);

            LastPoint = point;

            _polyline = new Polyline
            {
                StrokeThickness = canvas.DrawingStrokeThickness,
                Stroke = canvas.DrawingColor,
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
            LastPoint = null;

            var tmp = _polyline;

            _polyline = null;

            return tmp;
        }

        public override UIElement? OnMouseMove(MouseEventArgs e)
        {
            if (_polyline is null ||
                e.LeftButton != MouseButtonState.Pressed ||
                _liveShotService.DrawCanvas is not { } canvas
            ) return null;

            var point = e.GetPosition(canvas);

            LastPoint = point;

            _polyline.Points.Add(point);

            return null;
        }

        public override void UpdateThickness(double thickness)
        {
            if (_polyline is null) return;

            _polyline.StrokeThickness = thickness;
        }
    }
}