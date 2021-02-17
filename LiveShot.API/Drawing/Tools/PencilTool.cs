using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LiveShot.API.Canvas;

namespace LiveShot.API.Drawing.Tools
{
    public class PencilTool : DrawingTool
    {
        public override CanvasTool Tool => CanvasTool.Pencil;

        private Polyline? _polyline;

        public override void OnMouseLeftButtonDown(MouseButtonEventArgs e, AbstractDrawCanvas canvas)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

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

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e, AbstractDrawCanvas canvas)
        {
            LastPoint = null;
            _polyline = null;
        }

        public override void OnMouseMove(MouseEventArgs e, AbstractDrawCanvas canvas)
        {
            if (_polyline is null || e.LeftButton != MouseButtonState.Pressed) return;

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