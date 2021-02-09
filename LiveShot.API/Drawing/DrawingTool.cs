using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace LiveShot.API.Drawing
{
    public abstract class DrawingTool : IDrawingTool
    {
        protected Point? LastPoint;

        protected DrawingTool()
        {
            Color = Brushes.Transparent;
        }

        public Brush Color { get; set; }
        public abstract CanvasTool Tool { get; }

        public abstract void OnMouseLeftButtonDown(MouseButtonEventArgs e, System.Windows.Controls.Canvas canvas);
        public abstract void OnMouseLeftButtonUp(MouseButtonEventArgs e, System.Windows.Controls.Canvas canvas);
        public abstract void OnMouseMove(MouseEventArgs e, System.Windows.Controls.Canvas canvas);

    }
}