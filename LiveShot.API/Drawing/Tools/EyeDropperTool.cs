using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LiveShot.API.Canvas;
using LiveShot.API.Utils;

namespace LiveShot.API.Drawing.Tools
{
    public class EyeDropperTool : DrawingTool
    {
        public override CanvasTool Tool => CanvasTool.EyeDropper;

        private System.Windows.Controls.Image? _magnifier;

        public override void OnMouseLeftButtonDown(MouseButtonEventArgs e, AbstractDrawCanvas canvas) { }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e, AbstractDrawCanvas canvas)
        {
            if (canvas.ScreenShot is null)
                return;

            var point = e.GetPosition(canvas);
            var pixel = new byte[4];

            ImageUtils
                .GetBitmapSource(canvas.ScreenShot)
                .CopyPixels(new Int32Rect((int) point.X, (int) point.Y, 1, 1), pixel, 4, 0);

            canvas.UpdateDrawingColor(new SolidColorBrush(Color.FromRgb(pixel[2], pixel[1], pixel[0])));
        }

        public override void OnMouseMove(MouseEventArgs e, AbstractDrawCanvas canvas)
        {
            if (canvas.ScreenShot is null) return;

            var point = e.GetPosition(canvas);

            _magnifier ??= ImageUtils.GetMagnifiedImage(canvas.ScreenShot, point, 8, 4);
            _magnifier.Source = ImageUtils.GetMagnifiedBitmap(point, 8, canvas.ScreenShot);

            if (!canvas.Children.Contains(_magnifier))
            {
                canvas.Children.Insert(0, _magnifier);
            }
            
            System.Windows.Controls.Canvas.SetLeft(_magnifier, point.X - _magnifier.Width / 2);
            System.Windows.Controls.Canvas.SetTop(_magnifier, point.Y - _magnifier.Height / 2);
        }

        public override void Unselect()
        {
            (_magnifier?.Parent as AbstractDrawCanvas)?.Children.Remove(_magnifier);
        }
    }
}