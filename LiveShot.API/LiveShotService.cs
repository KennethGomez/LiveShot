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
        private IActionButton? _activeActionButton;
        private Bitmap? _screenShot;

        public System.Windows.Controls.Canvas? SelectCanvas { get; set; }
        public AbstractDrawCanvas? DrawCanvas { get; set; }

        public Bitmap? ScreenShot
        {
            get => _screenShot;
            set
            {
                _screenShot = value;

                if (_screenShot is not null)
                {
                    ScreenShotBytes = ImageUtils.GetBytes(_screenShot);
                }
            }
        }

        public byte[]? ScreenShotBytes { get; private set; }

        public IActionButton? ActiveActionButton
        {
            get => _activeActionButton;
            set
            {
                if (value is null)
                {
                    _activeActionButton?.UpdateIconFill(Brushes.Black);
                }
                
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