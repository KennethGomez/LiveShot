using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LiveShot.UI.Objects;

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

        private void UpdateSelection()
        {
            SetLeft(_selection.Rectangle, _selection.Left);
            SetTop(_selection.Rectangle, _selection.Top);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            bool exists = _selection != null;

            if (!exists)
            {
                _selection = Selection.Empty;

                Children.Add(_selection.Rectangle);
            }

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

            UpdateSelection();
        }

        public void ParentKeyDown(KeyEventArgs e)
        {
            if (_selection == null) return;

            OnSelectionKeyDown(e, Key.Up, n => _selection.Height -= n, n => _selection.Top -= n);
            OnSelectionKeyDown(e, Key.Right, n => _selection.Width += n, n => _selection.Left += n);
            OnSelectionKeyDown(e, Key.Down, n => _selection.Height += n, n => _selection.Top += n);
            OnSelectionKeyDown(e, Key.Left, n => _selection.Width -= n, n => _selection.Left -= n);

            UpdateSelection();
        }
    }
}