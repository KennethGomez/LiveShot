using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LiveShot.Objects;
using LiveShot.Objects.Events;
using LiveShot.Objects.Events.Input;

namespace LiveShot.UI.Controls
{
    public class SelectCanvas : Canvas
    {
        public static readonly DependencyProperty SizeLabelProperty = DependencyProperty.Register("SizeLabel",
            typeof(Label), typeof(SelectCanvas));

        private bool _dragging = true;
        private bool _moving;

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

        public Selection? Selection { get; private set; }

        public void WithEvents(IEventPipeline events)
        {
            events.Subscribe<OnKeyDown>(OnKeyDown);
        }

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
            if (Selection is null) return;

            SetLeft(Selection.Rectangle, Selection.Left);
            SetTop(Selection.Rectangle, Selection.Top);

            SizeLabel.Content = Selection.IsClear ? "Empty selection" : $"{Selection.Width} × {Selection.Height}";
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (Selection is null)
            {
                Selection = Selection.Empty;

                Children.Add(Selection.Rectangle);
            }

            if (e.ClickCount >= 2)
            {
                Selection.Clear();

                UpdateSelection();

                return;
            }

            var position = e.GetPosition(this);

            _moving = Selection.Contains(position);
            _dragging = !_moving;
            _startPosition = position;
            _tmpCursorPosition = _startPosition;

            if (!_moving) Selection.Cursor = Cursors.Arrow;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _dragging = false;
            _moving = false;
            _startPosition = null;
            _tmpCursorPosition = null;

            if (Selection is null) return;

            Selection.Cursor = Cursors.SizeAll;
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
            if (_startPosition is not { } startPosition || Selection is null) return;

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

        private void OnKeyDown(Event e)
        {
            if (Selection == null) return;

            switch (e.GetArgs<KeyEventArgs>().Key)
            {
                case Key.Up:
                    OnSelectionKeyDown(n => Selection.Height -= n, n => Selection.Top -= n);
                    break;
                case Key.Right:
                    OnSelectionKeyDown(n => Selection.Width += n, n => Selection.Left += n);
                    break;
                case Key.Down:
                    OnSelectionKeyDown(n => Selection.Height += n, n => Selection.Top += n);
                    break;
                case Key.Left:
                    OnSelectionKeyDown(n => Selection.Width -= n, n => Selection.Left -= n);
                    break;
            }

            UpdateSelection();
        }
    }
}