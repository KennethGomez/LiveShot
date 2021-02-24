using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LiveShot.API.Canvas;

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

        public override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || _liveShotService.DrawCanvas is not { } canvas) return;

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
        }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            LastPoint = null;
            _polyline = null;
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (_polyline is null ||
                e.LeftButton != MouseButtonState.Pressed ||
                _liveShotService.DrawCanvas is not { } canvas
            ) return;

            var point = e.GetPosition(canvas);

            LastPoint = point;

            _polyline.Points.Add(point);
        }

        public override void UpdateThickness(double thickness)
        {
            if (_polyline is null) return;

            _polyline.StrokeThickness = thickness;
        }
    }
}