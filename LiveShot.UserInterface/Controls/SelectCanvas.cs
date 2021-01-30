using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using LiveShot.UserInterface.Objects;

namespace LiveShot.UserInterface.Controls
{
    public class SelectCanvas : Canvas
    {
        public static readonly StyledProperty<Selection?> SelectionProperty =
            AvaloniaProperty.Register<SelectCanvas, Selection?>(nameof(Selection));

        private bool _dragging = true;
        private bool _moving;

        private Point? _startPosition;
        private Point? _tmpCursorPosition;

        public SelectCanvas()
        {
            Background = Brushes.Black;
            Opacity = .4;

            DoubleTappedEvent.AddClassHandler<SelectCanvas>((x, _) => x.OnDoubleTap());
        }

        private void OnDoubleTap()
        {
            Selection?.Clear();

            UpdateSelection();
        }

        private static bool CtrlPressed(KeyEventArgs e) => (e.KeyModifiers & KeyModifiers.Control) != 0;
        private static bool ShiftPressed(KeyEventArgs e) => (e.KeyModifiers & KeyModifiers.Shift) != 0;

        public Selection? Selection
        {
            get => GetValue(SelectionProperty);
            set => SetValue(SelectionProperty, value);
        }

        private static void OnSelectionKeyDown(KeyEventArgs e, Action<int> shiftPressed, Action<int> normal)
        {
            int step = CtrlPressed(e) ? 10 : 1;

            if (ShiftPressed(e))
                shiftPressed(step);
            else
                normal(step);
        }

        private void UpdateSelection()
        {
            if (Selection is null) return;

            SetLeft(Selection.Rectangle, Selection.Left);
            SetTop(Selection.Rectangle, Selection.Top);

        }
        
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if (Selection is null)
            {
                Selection = Selection.Empty;

                Children.Add(Selection.Rectangle);
            }

            var position = e.GetPosition(this);

            _moving = Selection.Contains(position);
            _dragging = !_moving;
            _startPosition = position;
            _tmpCursorPosition = _startPosition;

            if (!_moving) Selection.Cursor = new Cursor(StandardCursorType.Arrow);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            _dragging = false;
            _moving = false;
            _startPosition = null;
            _tmpCursorPosition = null;

            if (Selection is not null)
            {
                Selection.Cursor = new Cursor(StandardCursorType.SizeAll);
            }
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            var newPosition = e.GetPosition(this);

            if (_moving)
                MoveSelection(newPosition);
            else if (_dragging) ResizeSelection(newPosition);

            UpdateSelection();
        }


        private void ResizeSelection(Point cursorPosition)
        {
            if (Selection is null) return;
            
            if (_startPosition is not { } startPosition) return;

            double xDiff = cursorPosition.X - startPosition.X;
            double yDiff = cursorPosition.Y - startPosition.Y;

            bool growingX = xDiff > 0;
            bool growingY = yDiff > 0;

            double rectTop = growingY ? startPosition.Y : cursorPosition.Y;
            double rectLeft = growingX ? startPosition.X : cursorPosition.X;

            Selection.Left = (int) rectLeft;
            Selection.Top = (int) rectTop;
            Selection.Width = (int) Math.Abs(xDiff);
            Selection.Height = (int) Math.Abs(yDiff);
        }

        private void MoveSelection(Point cursorPosition)
        {
            if (Selection is null) return;
            
            if (_tmpCursorPosition is { } tmpPosition)
            {
                double xDiff = cursorPosition.X - tmpPosition.X;
                double yDiff = cursorPosition.Y - tmpPosition.Y;

                Selection.Left += xDiff;
                Selection.Top += yDiff;
            }

            _tmpCursorPosition = cursorPosition;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Selection is null) return;

            switch (e.Key)
            {
                case Key.Up:
                    OnSelectionKeyDown(e, n => Selection.Height -= n, n => Selection.Top -= n);
                    break;
                case Key.Right:
                    OnSelectionKeyDown(e, n => Selection.Width += n, n => Selection.Left += n);
                    break;
                case Key.Down:
                    OnSelectionKeyDown(e, n => Selection.Height += n, n => Selection.Top += n);
                    break;
                case Key.Left:
                    OnSelectionKeyDown(e, n => Selection.Width -= n, n => Selection.Left -= n);
                    break;
            }

            UpdateSelection();
        }
    }
}