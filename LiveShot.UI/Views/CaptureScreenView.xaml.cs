using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LiveShot.Objects;
using LiveShot.Objects.Events;
using LiveShot.Utils;

namespace LiveShot.UI.Views
{
    public partial class CaptureScreenView
    {
        private ExportWindowView? _exportWindow;
        private Bitmap? _screenShot;

        public CaptureScreenView()
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

        private static bool IsCtrlPressed => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

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
                case Key.D:
                    OpenExportWindow();
                    break;
            }

            EventPipeline.Dispatch(new OnKeyDown(e));
        }

        private void OpenExportWindow()
        {
            if (!IsCtrlPressed || _exportWindow is not null) return;

            var selection = SelectCanvas.Selection;

            if (selection == null || selection.IsClear) return;

            _exportWindow = new ExportWindowView();
            _exportWindow.Show();

            double x = Width - _exportWindow.Width - 100;
            double y = Height - _exportWindow.Height - 100;

            _exportWindow.Left = x;
            _exportWindow.Top = y;
        }

        private void CopyImage()
        {
            if (!IsCtrlPressed) return;

            var selection = SelectCanvas.Selection;
            if (selection is null || selection.IsClear) return;

            ImageUtils.CopyImage(selection, _screenShot);

            Close();
        }
    }
}