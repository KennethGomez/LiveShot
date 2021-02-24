using System.Windows;
using System.Windows.Input;

namespace LiveShot.API.Drawing
{
    public abstract class DrawingTool : IDrawingTool
    {
        protected Point? LastPoint;

        public abstract CanvasTool Tool { get; }

        public virtual void OnMouseLeftButtonDown(MouseButtonEventArgs e) { }
        public virtual void OnMouseLeftButtonUp(MouseButtonEventArgs e) { }
        public virtual void OnMouseMove(MouseEventArgs e) { }

        public virtual void UpdateThickness(double thickness) { }

        public virtual void Unselect() { }
    }
}