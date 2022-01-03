using System.Windows;
using System.Windows.Input;

namespace LiveShot.API.Drawing
{
    public abstract class DrawingTool : IDrawingTool
    {
        protected Point? LastPoint;

        public abstract CanvasTool Tool { get; }

        public virtual void Select() { }

        public virtual UIElement? OnMouseLeftButtonDown(MouseButtonEventArgs e) => null;
        public virtual UIElement? OnMouseLeftButtonUp(MouseButtonEventArgs e) => null;
        public virtual UIElement? OnMouseMove(MouseEventArgs e) => null;

        public virtual void UpdateThickness(double thickness) { }

        public virtual void Unselect() { }
    }
}