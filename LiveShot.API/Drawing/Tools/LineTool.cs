using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace LiveShot.API.Drawing.Tools
{
    public class LineTool : DrawingTool
    {
        private Line? _line;

        private readonly ILiveShotService _liveShotService;

        public LineTool(ILiveShotService liveShotService)
        {
            _liveShotService = liveShotService;
        }

        public override CanvasTool Tool => CanvasTool.Line;

        public override UIElement? OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || _liveShotService.DrawCanvas is not { } canvas) return null;

            var point = e.GetPosition(canvas);

            Line line = new()
            {
                Stroke = canvas.DrawingColor,
                X1 = point.X,
                X2 = point.X,
                Y1 = point.Y,
                Y2 = point.Y,
                StrokeThickness = canvas.DrawingStrokeThickness
            };

            canvas.Children.Add(line);

            _line = line;

            return null;
        }

        public override UIElement? OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            var tmp = _line;

            _line = null;

            return tmp;
        }

        public override UIElement? OnMouseMove(MouseEventArgs e)
        {
            if (_line is null || _liveShotService.DrawCanvas is not { } canvas) return null;

            var point = e.GetPosition(canvas);

            _line.X2 = point.X;
            _line.Y2 = point.Y;

            return null;
        }

        public override void UpdateThickness(double thickness)
        {
            if (_line is not null)
                _line.StrokeThickness = thickness;
        }
    }
}