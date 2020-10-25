using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LiveShot.Utils.Image;

namespace LiveShot.UI.Forms
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Bitmap _screenShot;

        public MainWindow()
        {
            InitializeComponent();

            Top = SystemParameters.VirtualScreenTop;
            Left = SystemParameters.VirtualScreenLeft;
            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;

            SelectCanvas.Width = Width;
            SelectCanvas.Height = Height;

            CaptureScreen();
        }

        private void CaptureScreen()
        {
            (int screenTop, int screenLeft, int screenWidth, int screenHeight) =
                ((int, int, int, int)) (Top, Left, Width, Height);

            Opacity = .0;

            var bitmap = ImageUtils.CaptureScreen(screenWidth, screenHeight, screenLeft, screenTop);
            var bitmapSource = ImageUtils.GetBitmapSource(bitmap);

            _screenShot = bitmap;

            Background = new ImageBrush(bitmapSource);

            Opacity = 1;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    break;
                case Key.C:
                    CopyImage();
                    break;
            }

            SelectCanvas.ParentKeyDown(e);
        }

        private void CopyImage()
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl)) return;

            var selection = SelectCanvas.Selection;

            if (!selection.IsClear)
            {
                ImageUtils.CopyImage(selection, _screenShot);

                Close();
            }
        }
    }
}