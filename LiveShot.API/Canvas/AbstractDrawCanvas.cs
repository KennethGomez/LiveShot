using System.Windows.Media;

namespace LiveShot.API.Canvas
{
    public abstract class AbstractDrawCanvas : System.Windows.Controls.Canvas
    {
        public abstract Brush DrawingColor { get; set; }
        public abstract System.Windows.Controls.Canvas DrawingCanvas { get; set; }

        public abstract void Undo();
    }
}