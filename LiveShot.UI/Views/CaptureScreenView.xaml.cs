using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LiveShot.API;
using LiveShot.API.Events.Input;
using LiveShot.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Brushes = System.Windows.Media.Brushes;

namespace LiveShot.UI.Views
{
    public partial class CaptureScreenView
    {
        private readonly IEventPipeline _events;
        private readonly IServiceProvider _services;

        private ExportWindowView? _exportWindow;
        private Bitmap? _screenShot;

        public CaptureScreenView(IEventPipeline events, IServiceProvider services)
        {
            InitializeComponent();

            _events = events;
            _services = services;

            Top = SystemParameters.VirtualScreenTop;
            Left = SystemParameters.VirtualScreenLeft;
            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;

            SelectCanvas.Width = Width;
            SelectCanvas.Height = Height;
            SelectCanvas.WithEvents(events);

            CaptureScreen();
        }

        private static bool IsCtrlPressed => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

        private void CaptureScreen()
        {
            (int screenTop, int screenLeft, int screenWidth, int screenHeight) =
                ((int, int, int, int)) (Top, Left, Width, Height);

            var bitmap = ImageUtils.CaptureScreen(screenWidth, screenHeight, screenLeft, screenTop);
            var bitmapSource = ImageUtils.GetBitmapSource(bitmap);

            _screenShot = bitmap;

            Background = Brushes.Transparent;
            SelectCanvas.Background = new ImageBrush(bitmapSource);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    break;
                case Key.C:
                    if (IsCtrlPressed)
                        CopyImage();
                    break;
                case Key.S:
                    if (IsCtrlPressed)
                        if (SaveImage())
                            Close();
                    break;
                case Key.D:
                    if (IsCtrlPressed)
                        if (OpenExportWindow())
                            Close();
                    break;
            }

            _events.Dispatch<OnKeyDown>(e);
        }

        private bool SaveImage()
        {
            if (_screenShot is null) return false;
            
            SaveFileDialog dialog = new()
            {
                Filter = "PNG|*.png|JPEG|*.jpeg|BMP|*.bmp",
                FileName = $"Screenshot {DateTime.Now:dd-MM-yyyy HH mm}",
                RestoreDirectory = true,
                Title = API.Properties.Resources.CaptureScreenView_SaveImage_Title
            };

            dialog.ShowDialog();

            if (string.IsNullOrWhiteSpace(dialog.FileName)) return false;

            try
            {
                FileStream fs = (FileStream) dialog.OpenFile();

                switch (dialog.FilterIndex)
                {
                    case 1:
                        _screenShot.Save(fs, ImageFormat.Png);
                        break;
                    case 2:
                        _screenShot.Save(fs, ImageFormat.Jpeg);
                        break;
                    case 3:
                        _screenShot.Save(fs, ImageFormat.Bmp);
                        break;
                }

                fs.Close();
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool OpenExportWindow()
        {
            if (_exportWindow is not null || _screenShot is null) return false;

            var selection = SelectCanvas.Selection;

            if (selection is null || selection.IsClear || selection.HasInvalidSize) return false;

            var bitmap = ImageUtils.GetBitmap(selection, _screenShot);

            _exportWindow = _services.GetService<ExportWindowView>();

            if (_exportWindow is null) return false;

            _exportWindow.Show();

            double x = Width - _exportWindow.Width - 100;
            double y = Height - _exportWindow.Height - 100;

            _exportWindow.Left = x;
            _exportWindow.Top = y;

            _exportWindow.Upload(bitmap);

            return true;
        }

        private void CopyImage()
        {
            var selection = SelectCanvas.Selection;
            if (selection is null || selection.IsClear) return;

            bool copied = ImageUtils.CopyImage(selection, _screenShot);

            if (copied) Close();
        }
    }
}