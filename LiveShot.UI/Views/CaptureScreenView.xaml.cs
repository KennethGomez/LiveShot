using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using LiveShot.API;
using LiveShot.API.Controls.ResizeMarker;
using LiveShot.API.Drawing;
using LiveShot.API.Events.Input;
using LiveShot.API.Events.Input.ResizeMarker;
using LiveShot.API.Utils;
using LiveShot.UI.Controls.Button;
using Microsoft.Extensions.DependencyInjection;
using Brushes = System.Windows.Media.Brushes;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Panel = System.Windows.Controls.Panel;

namespace LiveShot.UI.Views
{
    public partial class CaptureScreenView
    {
        private readonly IEventPipeline _events;
        private readonly IServiceProvider _services;
        private readonly ILiveShotService _liveShotService;

        private ExportWindowView? _exportWindow;
        private ActionButton? _activeTool;
        private Bitmap? _screenShot;

        public CaptureScreenView(
            IEventPipeline events,
            IServiceProvider services,
            IEnumerable<IDrawingTool> tools,
            ILiveShotService liveShotService
        )
        {
            InitializeComponent();

            _events = events;
            _services = services;
            _liveShotService = liveShotService;

            _liveShotService.SelectCanvas = SelectCanvas;
            _liveShotService.DrawCanvas = DrawingCanvas;

            Top = SystemParameters.VirtualScreenTop;
            Left = SystemParameters.VirtualScreenLeft;
            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;

            SelectCanvas.Width = Width;
            SelectCanvas.Height = Height;
            SelectCanvas.WithEvents(events);

            DrawingCanvas.With(tools, events);

            CanvasRightPanel.With(events, Width, Height);
            CanvasBottomPanel.With(events, Width, Height);

            var actionButtons = WindowUtils.FindVisualChildren<ActionButton>(SelectCanvas).ToList();

            foreach (var actionButton in actionButtons)
                actionButton.Click += (actual, _) => ActionButtonOnClick(actual, actionButtons);

            ColorPickerBtn.Click += ColorPickerBtnOnClick;
            UndoBtn.Click += UndoBtnOnClick;

            UploadBtn.Click += (_, _) => OpenExportWindow();
            GoogleBtn.Click += (_, _) => OpenExportWindow(true);
            CopyBtn.Click += (_, _) => CopyImage();
            SaveBtn.Click += (_, _) => SaveImage();
            CloseBtn.Click += (_, _) => Close();

            foreach (
                var resizeMark in new[]
                {
                    ResizeMarkTopLeft, ResizeMarkTop, ResizeMarkTopRight,
                    ResizeMarkLeft, ResizeMarkRight,
                    ResizeMarkBottomLeft, ResizeMarkBottom, ResizeMarkBottomRight
                }
            ) PrepareResizeMark(resizeMark);

            CaptureScreen();
        }

        private void PrepareResizeMark(Panel resizeMark)
        {
            resizeMark.MouseEnter += (sender, _) => _events.Dispatch<OnResizeMarkerMouseEnter>(sender);
            resizeMark.MouseLeave += (sender, _) => _events.Dispatch<OnResizeMarkerMouseLeave>(sender);

            resizeMark.Width = resizeMark.Height = 6;
            resizeMark.Background = ResizeMarkerGradient.Striped;
            resizeMark.Opacity = 1;

            resizeMark.SetValue(Panel.ZIndexProperty, 2);
        }

        private void UndoBtnOnClick(object sender, RoutedEventArgs e)
        {
            Undo();
        }

        private void ColorPickerBtnOnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new ColorDialog();

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            DrawingCanvas.DrawingColor = ColorUtils.GetBrushFromChannels(
                dialog.Color.R, dialog.Color.G, dialog.Color.B, dialog.Color.A
            );

            _activeTool?.UpdateIconFill(DrawingCanvas.DrawingColor);
        }

        private void ActionButtonOnClick(object sender, IEnumerable<ActionButton> all)
        {
            foreach (var button in all)
            {
                if (button == sender)
                {
                    button.IsActive = !button.IsActive;

                    DrawingCanvas.Tool = button.IsActive ? button.ActiveTool : CanvasTool.Default;
                    _liveShotService.ActiveActionButton = button;

                    _activeTool = button;

                    continue;
                }

                button.IsActive = false;
                button.UpdateIconFill(DrawingCanvas.DrawingColor);

                DrawingCanvas.UnselectTool(button.ActiveTool);
            }
        }

        private void Undo()
        {
            DrawingCanvas.Undo();
        }

        private void CaptureScreen()
        {
            (int screenTop, int screenLeft, int screenWidth, int screenHeight) =
                ((int, int, int, int))(Top, Left, Width, Height);

            var bitmap = ImageUtils.CaptureScreen(screenWidth, screenHeight, screenLeft, screenTop);
            var bitmapSource = ImageUtils.GetBitmapSource(bitmap);

            _screenShot = bitmap;

            SelectCanvas.Background = new ImageBrush(bitmapSource);
            _liveShotService.ScreenShot = bitmap;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    break;
                case Key.C:
                    if (KeyBoardUtils.IsCtrlPressed)
                        CopyImage();
                    break;
                case Key.S:
                    if (KeyBoardUtils.IsCtrlPressed)
                        SaveImage();
                    break;
                case Key.D:
                    if (KeyBoardUtils.IsCtrlPressed)
                        OpenExportWindow();
                    break;
                case Key.Z:
                    if (KeyBoardUtils.IsCtrlPressed)
                        Undo();
                    break;
            }

            _events.Dispatch<OnKeyDown>(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            _events.Dispatch<OnKeyUp>(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            _events.Dispatch<OnMouseWheel>(e);
        }

        private void SaveImage()
        {
            var selection = SelectCanvas.Selection;

            if (_screenShot is null || selection is null || selection.Invalid) return;

            DrawingCanvas.UnselectTool();

            bool saved = FileUtils.SaveImage(selection, _screenShot, ImageUtils.GetBitmapFromCanvas(DrawingCanvas));

            if (saved)
                Close();
        }

        private void OpenExportWindow(bool google = false)
        {
            if (_exportWindow is not null || _screenShot is null) return;

            var selection = SelectCanvas.Selection;

            if (selection is null || selection.Invalid) return;

            DrawingCanvas.UnselectTool();

            var bitmap = ImageUtils.GetBitmap(selection, _screenShot, ImageUtils.GetBitmapFromCanvas(DrawingCanvas));

            _exportWindow = _services.GetService<ExportWindowView>();

            if (_exportWindow is null) return;

            _exportWindow.Show();

            double x = Width - _exportWindow.Width - 100;
            double y = Height - _exportWindow.Height - 100;

            _exportWindow.Left = x;
            _exportWindow.Top = y;

            _exportWindow.Upload(bitmap, google);

            Close();
        }

        private void CopyImage()
        {
            var selection = SelectCanvas.Selection;
            if (selection is null || selection.Invalid || _screenShot is null) return;

            bool copied = ImageUtils.CopyImage(selection, _screenShot, ImageUtils.GetBitmapFromCanvas(SelectCanvas));

            if (copied) Close();
        }
    }
}