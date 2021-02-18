using System.Windows.Input;
using LiveShot.API.Canvas;

namespace LiveShot.API.Drawing
{
    public interface IDrawingTool
    {
        CanvasTool Tool { get; }

        void OnMouseLeftButtonDown(MouseButtonEventArgs e, AbstractDrawCanvas canvas);
        void OnMouseLeftButtonUp(MouseButtonEventArgs e, AbstractDrawCanvas canvas);
        void OnMouseMove(MouseEventArgs e, AbstractDrawCanvas canvas);
        void UpdateThickness(double thickness);
        void Unselect();
    }
}