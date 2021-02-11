using System.Windows;
using System.Windows.Input;
using LiveShot.API.Canvas;

namespace LiveShot.API.Drawing
{
    public abstract class DrawingTool : IDrawingTool
    {
        protected Point? LastPoint;

        public abstract CanvasTool Tool { get; }

        public abstract void OnMouseLeftButtonDown(MouseButtonEventArgs e, AbstractDrawCanvas canvas);
        public abstract void OnMouseLeftButtonUp(MouseButtonEventArgs e, AbstractDrawCanvas canvas);
        public abstract void OnMouseMove(MouseEventArgs e, AbstractDrawCanvas canvas);

        public virtual void UpdateThickness(int thickness)
        {
            return;
        }
    }
}