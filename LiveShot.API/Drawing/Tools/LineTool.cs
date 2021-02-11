using System.Windows.Input;
using System.Windows.Shapes;
using LiveShot.API.Canvas;

namespace LiveShot.API.Drawing.Tools
{
    public class LineTool : DrawingTool
    {
        private Line? _line;

        public override CanvasTool Tool => CanvasTool.Line;

        public override void OnMouseLeftButtonDown(MouseButtonEventArgs e, AbstractDrawCanvas canvas)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            var point = e.GetPosition(canvas);

            Line line = new()
            {
                Stroke = canvas.DrawingColor,
                X1 = point.X,
                X2 = point.X,
                Y1 = point.Y,
                Y2 = point.Y
            };

            canvas.Children.Add(line);

            _line = line;
        }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e, AbstractDrawCanvas canvas)
        {
            _line = null;
        }

        public override void OnMouseMove(MouseEventArgs e, AbstractDrawCanvas canvas)
        {
            if (_line is null) return;

            var point = e.GetPosition(canvas);

            _line.X2 = point.X;
            _line.Y2 = point.Y;
        }
    }
}