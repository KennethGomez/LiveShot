using System.Windows.Input;
using System.Windows.Media;
using LiveShot.API.Drawing;

namespace LiveShot.API.Canvas
{
    public abstract class AbstractDrawCanvas : System.Windows.Controls.Canvas
    {
        public abstract Brush DrawingColor { get; set; }
        public abstract Cursor DrawingCursor { get; }
        public abstract double DrawingStrokeThickness { get; set; }

        public abstract CanvasTool Tool { get; set; }
    }
}