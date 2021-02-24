using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LiveShot.API.Canvas;

namespace LiveShot.API.Drawing.Tools
{
    public class EyeDropperTool : DrawingTool
    {
        public override CanvasTool Tool => CanvasTool.EyeDropper;

        private readonly Rectangle?[,] _magnifier = new Rectangle[MagnifierSize * 2 + 1, MagnifierSize * 2 + 1];

        private const int MagnifierSize = 3;
        private const int ScreenShotStride = 4;

        public override void OnMouseLeftButtonDown(MouseButtonEventArgs e, AbstractDrawCanvas canvas)
        {
        }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e, AbstractDrawCanvas canvas)
        {
        }

        public override void OnMouseMove(MouseEventArgs e, AbstractDrawCanvas canvas)
        {
            if (canvas.ScreenShot is null || canvas.ScreenShotBytes is null) return;

            var point = e.GetPosition(canvas);

            var pixelIdx = (int) (
                (point.X - MagnifierSize) * ScreenShotStride + 
                (point.Y - MagnifierSize) * canvas.ScreenShot.Width * ScreenShotStride
            );

            for (var i = 0; i < _magnifier.GetLength(0); i++)
            {
                for (var j = 0; j < _magnifier.GetLength(1); j++)
                {
                    int rectIdx = pixelIdx + 
                                  i * ScreenShotStride + 
                                  j * ScreenShotStride * canvas.ScreenShot.Width;

                    if (rectIdx + ScreenShotStride > canvas.ScreenShotBytes.Length || rectIdx < 0)
                        continue;

                    byte b = canvas.ScreenShotBytes[rectIdx];
                    byte g = canvas.ScreenShotBytes[rectIdx + 1];
                    byte r = canvas.ScreenShotBytes[rectIdx + 2];

                    if (_magnifier[i, j] is not { } rect)
                    {
                        rect = new Rectangle
                        {
                            Width = MagnifierSize * 2,
                            Height = MagnifierSize * 2
                        };

                        _magnifier[i, j] = rect;

                        rect.MouseUp += (sender, _) => OnRectangleMouseUp(sender, canvas);

                        canvas.Children.Insert(0, rect);
                    }

                    rect.Fill = new SolidColorBrush(Color.FromRgb(r, g, b));


                    double left = point.X + (i - (MagnifierSize + 1)) * MagnifierSize * 2;
                    double top = point.Y + (j - (MagnifierSize + 1)) * MagnifierSize * 2;

                    System.Windows.Controls.Canvas.SetLeft(rect, left);
                    System.Windows.Controls.Canvas.SetTop(rect, top);
                }
            }
        }

        private void RemoveMagnifier()
        {
            for (var i = 0; i < _magnifier.GetLength(0); i++)
            {
                for (var j = 0; j < _magnifier.GetLength(1); j++)
                {
                    if (_magnifier[i, j] is not {Parent: AbstractDrawCanvas canvas})
                        continue;

                    canvas.Children.Remove(_magnifier[i, j]);

                    _magnifier[i, j] = null;
                }
            }
        }

        private void OnRectangleMouseUp(object sender, AbstractDrawCanvas canvas)
        {
            var rect = (Rectangle) sender;

            canvas.UpdateDrawingColor(rect.Fill);
        }

        public override void Unselect()
        {
            RemoveMagnifier();
        }
    }
}