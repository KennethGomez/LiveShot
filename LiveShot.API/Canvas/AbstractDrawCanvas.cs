using System.Drawing;
using System.Windows.Input;
using LiveShot.API.Controls.Button;
using LiveShot.API.Drawing;
using Brush = System.Windows.Media.Brush;

namespace LiveShot.API.Canvas
{
    public abstract class AbstractDrawCanvas : System.Windows.Controls.Canvas
    {
        public abstract Brush DrawingColor { get; set; }
        public abstract Cursor DrawingCursor { get; }
        public abstract double DrawingStrokeThickness { get; set; }
        public abstract CanvasTool Tool { get; set; }
        public abstract Bitmap? ScreenShot { get; set; }
        public abstract byte[]? ScreenShotBytes { get; }
        public abstract IActionButton? ActiveActionButton { get; set; }

        public abstract void Undo();
        public abstract void UpdateDrawingColor(Brush brush);
    }
}