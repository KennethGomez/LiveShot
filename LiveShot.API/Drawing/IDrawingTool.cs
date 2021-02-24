using System.Windows.Input;

namespace LiveShot.API.Drawing
{
    public interface IDrawingTool
    {
        CanvasTool Tool { get; }

        void OnMouseLeftButtonDown(MouseButtonEventArgs e);
        void OnMouseLeftButtonUp(MouseButtonEventArgs e);
        void OnMouseMove(MouseEventArgs e);
        void UpdateThickness(double thickness);
        void Unselect();
    }
}