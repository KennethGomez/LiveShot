using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LiveShot.API.Canvas;

namespace LiveShot.API.Drawing.Tools
{
    public class HighlightTool : DrawingTool
    {
        public static readonly Brush Color = Brushes.Yellow;
        public const double Opacity = 0.5;
        
        public override CanvasTool Tool => CanvasTool.Highlight;

        private Polyline? _polyline;

        public override void OnMouseLeftButtonDown(MouseButtonEventArgs e, AbstractDrawCanvas canvas)
        {
            if (e.ButtonState != MouseButtonState.Pressed) return;

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
        }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e, AbstractDrawCanvas canvas)
        {
            _polyline = null;
        }

        public override void OnMouseMove(MouseEventArgs e, AbstractDrawCanvas canvas)
        {
            if (_polyline is null) return;

            var point = e.GetPosition(canvas);

            LastPoint = point;
            
            _polyline.Points.Add(point);
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