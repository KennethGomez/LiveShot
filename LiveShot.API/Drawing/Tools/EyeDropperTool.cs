using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LiveShot.API.Canvas;

namespace LiveShot.API.Drawing.Tools
{
    public class EyeDropperTool : DrawingTool
    {
        private readonly Rectangle?[,] _magnifier = new Rectangle[MagnifierSize * 2 + 1, MagnifierSize * 2 + 1];

        private const int MagnifierSize = 3;
        private const int ScreenShotStride = 4;

        private readonly ILiveShotService _liveShotService;

        public EyeDropperTool(ILiveShotService liveShotService)
        {
            _liveShotService = liveShotService;
        }

        public override CanvasTool Tool => CanvasTool.EyeDropper;

        public override UIElement? OnMouseMove(MouseEventArgs e)
        {
            if (_liveShotService.ScreenShot is null ||
                _liveShotService.ScreenShotBytes is null ||
                _liveShotService.DrawCanvas is not { } canvas
            ) return null;

            var point = e.GetPosition(canvas);

            var pixelIdx = (int) (
                (point.X - MagnifierSize) * ScreenShotStride +
                (point.Y - MagnifierSize) * _liveShotService.ScreenShot.Width * ScreenShotStride
            );

            for (var i = 0; i < _magnifier.GetLength(0); i++)
            {
                for (var j = 0; j < _magnifier.GetLength(1); j++)
                {
                    int rectIdx = pixelIdx +
                                  i * ScreenShotStride +
                                  j * ScreenShotStride * _liveShotService.ScreenShot.Width;

                    if (rectIdx + ScreenShotStride > _liveShotService.ScreenShotBytes.Length || rectIdx < 0)
                        continue;

                    byte b = _liveShotService.ScreenShotBytes[rectIdx];
                    byte g = _liveShotService.ScreenShotBytes[rectIdx + 1];
                    byte r = _liveShotService.ScreenShotBytes[rectIdx + 2];

                    if (_magnifier[i, j] is not { } rect)
                    {
                        rect = new Rectangle
                        {
                            Width = MagnifierSize * 2,
                            Height = MagnifierSize * 2
                        };

                        _magnifier[i, j] = rect;

                        rect.MouseUp += OnRectangleMouseUp;

                        canvas.Children.Add(rect);
                    }

                    rect.Fill = new SolidColorBrush(Color.FromRgb(r, g, b));


                    double left = point.X + (i - (MagnifierSize + 1)) * MagnifierSize * 2;
                    double top = point.Y + (j - (MagnifierSize + 1)) * MagnifierSize * 2;

                    System.Windows.Controls.Canvas.SetLeft(rect, left);
                    System.Windows.Controls.Canvas.SetTop(rect, top);
                }
            }

            return null;
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

        private void OnRectangleMouseUp(object sender, MouseButtonEventArgs _)
        {
            var rect = (Rectangle) sender;

            _liveShotService.UpdateDrawingColor(rect.Fill);
        }

        public override void Unselect()
        {
            RemoveMagnifier();
        }
    }
}