using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LiveShot.API;
using LiveShot.API.Events;
using LiveShot.API.Events.Input;
using LiveShot.API.Events.Window;
using LiveShot.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace LiveShot.UI.Views
{
    public partial class CaptureScreenView
    {
        private readonly IServiceProvider _services;
        private readonly IEventPipeline _events;
        
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

            _events.Subscribe<OnClosed>(OnWindowClosed);
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

            _events.Dispatch<OnKeyDown>(e);
        }

        private void OpenExportWindow()
        {
            if (!IsCtrlPressed || _exportWindow is not null) return;

            var selection = SelectCanvas.Selection;

            if (selection == null || selection.IsClear) return;

            _exportWindow = _services.GetService<ExportWindowView>();
            
            if (_exportWindow is null) return;
            
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

        private void OnWindowClosed(Event e)
        {
            if (e.GetArgs<OnClosedArgs>().Window == _exportWindow)
            {
                _exportWindow = null;

                Close();
            }
        }

        private new void Close()
        {
            _exportWindow?.Close();

            base.Close();
        }
    }
}