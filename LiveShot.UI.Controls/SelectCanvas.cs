using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using LiveShot.API;
using LiveShot.API.Canvas;
using LiveShot.API.Events;
using LiveShot.API.Events.Input;
using LiveShot.API.Events.Selection;
using LiveShot.UI.Controls.Panel;

namespace LiveShot.UI.Controls
{
    public class SelectCanvas : Canvas
    {
        public static readonly DependencyProperty SizeLabelProperty = DependencyProperty.Register(
            "SizeLabel", typeof(Label), typeof(SelectCanvas)
        );

        public static readonly DependencyProperty OpacityRectangleProperty = DependencyProperty.Register(
            "OpacityRectangle", typeof(Rectangle), typeof(SelectCanvas)
        );

        public static readonly DependencyProperty RightPanelProperty = DependencyProperty.Register(
            "RightPanel", typeof(RightDragPanel), typeof(SelectCanvas)
        );

        public static readonly DependencyProperty BottomPanelProperty = DependencyProperty.Register(
            "BottomPanel", typeof(BottomDragPanel), typeof(SelectCanvas)
        );

        private readonly Collection<Rectangle> _rectangles = new();

        private bool _dragging = true;

        private IEventPipeline? _events;
        private bool _moving;
        private Point? _startPosition;
        private Point? _tmpCursorPosition;

        public Label SizeLabel
        {
            get => (Label) GetValue(SizeLabelProperty);
            set => SetValue(SizeLabelProperty, value);
        }

        public Rectangle OpacityRectangle
        {
            get => (Rectangle) GetValue(OpacityRectangleProperty);
            set => SetValue(OpacityRectangleProperty, value);
        }

        public RightDragPanel RightPanel
        {
            get => (RightDragPanel) GetValue(RightPanelProperty);
            set => SetValue(RightPanelProperty, value);
        }

        public BottomDragPanel BottomPanel
        {
            get => (BottomDragPanel) GetValue(BottomPanelProperty);
            set => SetValue(BottomPanelProperty, value);
        }

        private static bool CtrlPressed => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        private static bool ShiftPressed => Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

        public Selection? Selection { get; private set; }

        public void WithEvents(IEventPipeline events)
        {
            _events = events;

            _events.Subscribe<OnKeyDown>(OnKeyDown);
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
            if (Selection is null)
            {
                OpacityRectangle.Visibility = Visibility.Visible;

                return;
            }

            UpdateOpacityRectangles();

            SetLeft(Selection.Rectangle, Selection.Left);
            SetTop(Selection.Rectangle, Selection.Top);

            SizeLabel.Content = Selection.Label;

            if (Selection.HasInvalidSize || _dragging)
            {
                RightPanel.Visibility = Visibility.Hidden;
                BottomPanel.Visibility = Visibility.Hidden;
            }
            else
            {
                RightPanel.Visibility = Visibility.Visible;
                BottomPanel.Visibility = Visibility.Visible;
            }

            _events?.Dispatch<OnSelectionChange>(OnSelectionChangeArgs.From(Selection));
        }

        private void UpdateOpacityRectangles()
        {
            if (Selection is null) return;

            OpacityRectangle.Visibility = Visibility.Hidden;

            foreach (var rectangle in _rectangles) Children.Remove(rectangle);

            _rectangles.Clear();

            foreach (var bound in RectangleBounds.GetBounds(Selection, Width, Height))
            {
                (var rectangle, double left, double top) = bound;

                rectangle.MouseLeftButtonUp += RectangleOnMouseLeftButtonUp;
                rectangle.MouseLeftButtonDown += RectangleOnMouseLeftButtonDown;

                Children.Add(rectangle);

                SetLeft(rectangle, left);
                SetTop(rectangle, top);

                _rectangles.Add(rectangle);
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            MouseLeftButtonPress(e);
        }

        private void RectangleOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseLeftButtonPress(e);
        }

        private void MouseLeftButtonPress(MouseButtonEventArgs e)
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
            MouseLeftButtonRelease();
        }

        private void RectangleOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MouseLeftButtonRelease();
        }

        private void MouseLeftButtonRelease()
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