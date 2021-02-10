using System.Windows.Input;
using System.Windows.Shapes;

namespace LiveShot.API.Drawing.Tools
{
    public class LineTool : DrawingTool
    {
        private Line? _line;

        public override CanvasTool Tool => CanvasTool.Line;

        public override void OnMouseLeftButtonDown(MouseButtonEventArgs e, System.Windows.Controls.Canvas canvas)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            var point = e.GetPosition(canvas);

            Line line = new()
            {
                Stroke = Color,
                X1 = point.X,
                X2 = point.X,
                Y1 = point.Y,
                Y2 = point.Y
            };

            canvas.Children.Add(line);

            _line = line;
        }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e, System.Windows.Controls.Canvas canvas)
        {
            _line = null;
        }

        public override void OnMouseMove(MouseEventArgs e, System.Windows.Controls.Canvas canvas)
        {
            if (_line is null) return;

            var point = e.GetPosition(canvas);

            _line.X2 = point.X;
            _line.Y2 = point.Y;
        }
    }
}