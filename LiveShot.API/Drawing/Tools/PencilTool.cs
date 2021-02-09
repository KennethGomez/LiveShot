using System.Windows.Input;
using System.Windows.Shapes;

namespace LiveShot.API.Drawing.Tools
{
    public class PencilTool : DrawingTool
    {
        public override CanvasTool Tool => CanvasTool.Pencil;

        public override void OnMouseLeftButtonDown(MouseButtonEventArgs e, System.Windows.Controls.Canvas canvas)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            LastPoint = e.GetPosition(canvas);
        }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e, System.Windows.Controls.Canvas canvas)
        {
            LastPoint = null;
        }

        public override void OnMouseMove(MouseEventArgs e, System.Windows.Controls.Canvas canvas)
        {
            if (LastPoint is not { } lastPoint || e.LeftButton != MouseButtonState.Pressed) return;

            Line line = new()
            {
                Stroke = Color,
                X1 = lastPoint.X,
                Y1 = lastPoint.Y,
                X2 = e.GetPosition(canvas).X,
                Y2 = e.GetPosition(canvas).Y
            };

            LastPoint = e.GetPosition(canvas);

            canvas.Children.Add(line);
        }
    }
}