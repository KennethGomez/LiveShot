using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LiveShot.UI.Objects;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;
using Point = System.Windows.Point;

namespace LiveShot.UI.Controls
{
    public class SelectCanvas : Canvas
    {
        private bool _dragging = true;

        private Selection _selection;
        private Point? _startPosition;

        public SelectCanvas()
        {
            Background = Brushes.Black;
            Opacity = .4;
        }

        private static void OnSelectionKeyDown(KeyEventArgs e, Key key, Action<int> shiftPressed, Action<int> normal)
        {
            if (e.Key != key) return;

            bool controlDown = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            int step = controlDown ? 10 : 1;

            bool shiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (shiftDown)
                shiftPressed(step);
            else
                normal(step);
        }

        private void UpdateOpacityMask()
        {
            (int screenWidth, int screenHeight) = ((int, int)) (Width, Height);

            using var bitmap = new Bitmap((int) Width, (int) Height);
            using var graphics = Graphics.FromImage(bitmap);

            graphics.Clear(Color.Black);

            var path = new GraphicsPath();
            path.AddRectangle(_selection.Rectangle);

            graphics.SetClip(path);
            graphics.Clear(Color.Transparent);
            graphics.ResetClip();

            var source = Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(screenWidth, screenHeight)
            );

            OpacityMask = new ImageBrush(source);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            bool exists = _selection != null;

            if (!exists) _selection = Selection.Empty;

            _dragging = true;
            _startPosition = e.GetPosition(this);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _dragging = false;
            _startPosition = null;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!_dragging || _startPosition is not { } startPosition) return;

            var newPosition = e.GetPosition(this);

            double xDiff = newPosition.X - startPosition.X;
            double yDiff = newPosition.Y - startPosition.Y;

            bool growingX = xDiff > 0;
            bool growingY = yDiff > 0;

            double rectTop = growingY ? startPosition.Y : newPosition.Y;
            double rectLeft = growingX ? startPosition.X : newPosition.X;

            _selection.Left = (int) rectLeft;
            _selection.Top = (int) rectTop;
            _selection.Width = (int) Math.Abs(xDiff);
            _selection.Height = (int) Math.Abs(yDiff);

            UpdateOpacityMask();
        }

        public void ParentKeyDown(KeyEventArgs e)
        {
            if (_selection == null) return;

            OnSelectionKeyDown(e, Key.Up, n => _selection.Height -= n, n => _selection.Top -= n);
            OnSelectionKeyDown(e, Key.Right, n => _selection.Width += n, n => _selection.Left += n);
            OnSelectionKeyDown(e, Key.Down, n => _selection.Height += n, n => _selection.Top += n);
            OnSelectionKeyDown(e, Key.Left, n => _selection.Width -= n, n => _selection.Left -= n);

            UpdateOpacityMask();
        }
    }
}