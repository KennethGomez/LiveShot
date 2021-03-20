using System.Drawing;
using LiveShot.API.Canvas;
using LiveShot.API.Controls.Button;
using Brush = System.Windows.Media.Brush;

namespace LiveShot.API
{
    public interface ILiveShotService
    {
        System.Windows.Controls.Canvas? SelectCanvas { get; set; }
        AbstractDrawCanvas? DrawCanvas { get; set; }
        Bitmap? ScreenShot { get; set; }
        byte[]? ScreenShotBytes { get; }
        IActionButton? ActiveActionButton { get; set; }

        public void UpdateDrawingColor(Brush brush);
    }
}