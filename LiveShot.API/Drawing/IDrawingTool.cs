using System.Windows.Input;
using System.Windows.Media;

namespace LiveShot.API.Drawing
{
    public interface IDrawingTool
    {
        CanvasTool Tool { get; }
        Brush Color { get; set; }

        void OnMouseLeftButtonDown(MouseButtonEventArgs e, System.Windows.Controls.Canvas canvas);
        void OnMouseLeftButtonUp(MouseButtonEventArgs e, System.Windows.Controls.Canvas canvas);
        void OnMouseMove(MouseEventArgs e, System.Windows.Controls.Canvas canvas);
    }
}