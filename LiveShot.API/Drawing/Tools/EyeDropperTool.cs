using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LiveShot.API.Canvas;
using LiveShot.API.Utils;

namespace LiveShot.API.Drawing.Tools
{
    public class EyeDropperTool : DrawingTool
    {
        private Rectangle[,]? _magnifier;
        private Grid? _magnifierGrid;

        private const int MagnifierSize = 3;
        private const int MagnifierRectangleSize = MagnifierSize * 2;
        private const int MagnifierRealSize = MagnifierSize * 2 + 1;
        private const int ScreenShotStride = 4;

        private readonly ILiveShotService _liveShotService;

        public EyeDropperTool(ILiveShotService liveShotService)
        {
            _liveShotService = liveShotService;
        }

        public override void Select()
        {
            _magnifier = GetMagnifier();
            _magnifierGrid = GetMagnifierGrid();

            InitMagnifier();
        }

        private void InitMagnifier()
        {
            if (_magnifier is null || _magnifierGrid is null) return;
            
            for (var i = 0; i < _magnifier.GetLength(0); i++)
            {
                for (var j = 0; j < _magnifier.GetLength(1); j++)
                {
                    _magnifier[i, j].MouseUp += OnRectangleMouseUp;

                    _magnifierGrid.Children.Add(_magnifier[i, j]);

                    Grid.SetRow(_magnifier[i, j], j);
                    Grid.SetColumn(_magnifier[i, j], i);
                }
            }
        }

        private Rectangle[,] GetMagnifier()
        {
            var magnifier = new Rectangle[MagnifierRealSize, MagnifierRealSize];

            for (var i = 0; i < magnifier.GetLength(0); i++)
            {
                for (var j = 0; j < magnifier.GetLength(1); j++)
                {
                    magnifier[i, j] = new Rectangle
                    {
                        Width = MagnifierSize * 2,
                        Height = MagnifierSize * 2
                    };
                }
            }

            return magnifier;
        }

        private Grid GetMagnifierGrid()
        {
            Grid grid = new();

            for (var i = 0; i < MagnifierRealSize; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(MagnifierRectangleSize)
                });

                grid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(MagnifierRectangleSize)
                });
            }

            return grid;
        }

        public override CanvasTool Tool => CanvasTool.EyeDropper;

        public override UIElement? OnMouseMove(MouseEventArgs e)
        {
            if (_liveShotService.ScreenShot is null ||
                _liveShotService.ScreenShotBytes is null ||
                _liveShotService.DrawCanvas is not { } drawCanvas ||
                _magnifier is null || 
                _magnifierGrid is null
               ) return null;

            if (!drawCanvas.Children.Contains(_magnifierGrid))
                drawCanvas.Children.Add(_magnifierGrid);

            var point = e.GetPosition(drawCanvas);

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

                    _magnifier[i, j].Fill = new SolidColorBrush(Color.FromRgb(r, g, b));
                }
            }

            double left = Math.Floor(point.X - _magnifierGrid.ActualWidth / 2);
            double top = Math.Floor(point.Y - _magnifierGrid.ActualHeight / 2);

            System.Windows.Controls.Canvas.SetLeft(_magnifierGrid, left);
            System.Windows.Controls.Canvas.SetTop(_magnifierGrid, top);

            return null;
        }

        private void RemoveMagnifier()
        {
            if (_magnifierGrid is not {Parent: System.Windows.Controls.Canvas canvas})
                return;

            canvas.Children.Remove(_magnifierGrid);

            _magnifier = null;
            _magnifierGrid = null;
        }

        private void OnRectangleMouseUp(object sender, MouseButtonEventArgs _)
        {
            var rect = (Rectangle) sender;

            _liveShotService.UpdateDrawingColor(rect.Fill);

            if (KeyBoardUtils.IsShiftPressed && rect.Fill is SolidColorBrush brush)
            {
                Clipboard.SetText(brush.Color.ToHex());
            }
        }

        public override void Unselect()
        {
            RemoveMagnifier();
        }
    }
}