using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LiveShot.Objects;

namespace LiveShot.UI.Controls
{
    public class SelectCanvas : Canvas
    {
        public static readonly DependencyProperty SizeLabelProperty = DependencyProperty.Register("SizeLabel",
            typeof(Label), typeof(SelectCanvas));

        private bool _dragging = true;
        private bool _moving;

        private Selection? _selection;
        private Point? _startPosition;
        private Point? _tmpCursorPosition;

        public SelectCanvas()
        {
            Background = Brushes.Black;
            Opacity = .4;
        }

        public Label SizeLabel
        {
            get => (Label) GetValue(SizeLabelProperty);
            set => SetValue(SizeLabelProperty, value);
        }

        private static bool CtrlPressed => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        private static bool ShiftPressed => Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

        public Selection? Selection => _selection;

        private static void OnSelectionKeyDown(Action<int> shiftPressed, Action<int> normal)
        {
            int step = CtrlPressed ? 10 : 1;

            if (ShiftPressed)
                shiftPressed(step);
            else
                normal(step);
        }

        private void UpdateSelection()
        {
            if (_selection is null) return;

            SetLeft(_selection.Rectangle, _selection.Left);
            SetTop(_selection.Rectangle, _selection.Top);

            SizeLabel.Content = _selection.IsClear ? "Empty selection" : $"{_selection.Width} × {_selection.Height}";
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (_selection is null)
            {
                _selection = Selection.Empty;

                Children.Add(_selection.Rectangle);
            }

            if (e.ClickCount >= 2)
            {
                _selection.Clear();

                UpdateSelection();

                return;
            }

            var position = e.GetPosition(this);

            _moving = _selection.Contains(position);
            _dragging = !_moving;
            _startPosition = position;
            _tmpCursorPosition = _startPosition;

            if (!_moving) _selection.Cursor = Cursors.Arrow;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _dragging = false;
            _moving = false;
            _startPosition = null;
            _tmpCursorPosition = null;

            if (_selection is null) return;

            _selection.Cursor = Cursors.SizeAll;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            var newPosition = e.GetPosition(this);

            if (_moving)
                MoveSelection(newPosition);
            else if (_dragging) ResizeSelection(newPosition);

            UpdateSelection();
        }

        private void ResizeSelection(Point cursorPosition)
        {
            if (_startPosition is not {} startPosition || _selection is null) return;

            double xDiff = cursorPosition.X - startPosition.X;
            double yDiff = cursorPosition.Y - startPosition.Y;

            bool growingX = xDiff > 0;
            bool growingY = yDiff > 0;

            double rectTop = growingY ? startPosition.Y : cursorPosition.Y;
            double rectLeft = growingX ? startPosition.X : cursorPosition.X;

            _selection.Left = (int) rectLeft;
            _selection.Top = (int) rectTop;
            _selection.Width = (int) Math.Abs(xDiff);
            _selection.Height = (int) Math.Abs(yDiff);
        }

        private void MoveSelection(Point cursorPosition)
        {
            if (_selection is null) return;
            if (_tmpCursorPosition is {} tmpPosition)
            {
                double xDiff = cursorPosition.X - tmpPosition.X;
                double yDiff = cursorPosition.Y - tmpPosition.Y;

                _selection.Left += xDiff;
                _selection.Top += yDiff;
            }

            _tmpCursorPosition = cursorPosition;
        }

        public void ParentKeyDown(KeyEventArgs e)
        {
            if (_selection == null) return;

            switch (e.Key)
            {
                case Key.Up:
                    OnSelectionKeyDown(n => _selection.Height -= n, n => _selection.Top -= n);
                    break;
                case Key.Right:
                    OnSelectionKeyDown(n => _selection.Width += n, n => _selection.Left += n);
                    break;
                case Key.Down:
                    OnSelectionKeyDown(n => _selection.Height += n, n => _selection.Top += n);
                    break;
                case Key.Left:
                    OnSelectionKeyDown(n => _selection.Width -= n, n => _selection.Left -= n);
                    break;
            }

            UpdateSelection();
        }
    }
}