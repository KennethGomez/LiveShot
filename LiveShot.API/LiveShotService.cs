using System.Drawing;
using LiveShot.API.Canvas;
using LiveShot.API.Controls.Button;
using LiveShot.API.Utils;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace LiveShot.API
{
    public class LiveShotService : ILiveShotService
    {
        private byte[]? _screenShotBytes;
        private IActionButton? _activeActionButton;

        public AbstractDrawCanvas? DrawCanvas { get; set; }

        public Bitmap? ScreenShot { get; set; }

        public byte[]? ScreenShotBytes
        {
            get
            {
                if (ScreenShot is not { } screenShot) return null;

                return _screenShotBytes ??= ImageUtils.GetBytes(screenShot);
            }
        }

        public IActionButton? ActiveActionButton
        {
            get => _activeActionButton;
            set
            {
                _activeActionButton = value;
                _activeActionButton?.UpdateIconFill(DrawCanvas?.DrawingColor ?? Brushes.Black);
            }
        }

        public void UpdateDrawingColor(Brush brush)
        {
            if (DrawCanvas is null) 
                return;
            
            DrawCanvas.DrawingColor = brush;

            ActiveActionButton?.UpdateIconFill(brush);
        }
    }
}