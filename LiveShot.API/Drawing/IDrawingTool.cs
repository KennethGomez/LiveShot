using System.Windows;
using System.Windows.Input;

namespace LiveShot.API.Drawing
{
    public interface IDrawingTool
    {
        CanvasTool Tool { get; }

        UIElement? OnMouseLeftButtonDown(MouseButtonEventArgs e);
        UIElement? OnMouseLeftButtonUp(MouseButtonEventArgs e);
        UIElement? OnMouseMove(MouseEventArgs e);
        void UpdateThickness(double thickness);
        void Unselect();
    }
}