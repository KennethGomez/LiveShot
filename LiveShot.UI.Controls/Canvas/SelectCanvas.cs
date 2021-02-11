using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using LiveShot.API;
using LiveShot.API.Canvas;
using LiveShot.API.Drawing;
using LiveShot.API.Events;
using LiveShot.API.Events.Input;
using LiveShot.API.Events.Selection;
using LiveShot.API.Utils;
using LiveShot.UI.Controls.Panel;

namespace LiveShot.UI.Controls.Canvas
{
    public class SelectCanvas : DrawCanvas
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
                ClearOpacityRectangles();

                OpacityRectangle.Visibility = Visibility.Visible;

                SizeLabel.Content = API.Properties.Resources.CaptureScreen_SizeLabel_Empty;

                SetPanelsVisibility(Visibility.Hidden);

                return;
            }

            SetLeft(Selection.Rectangle, Selection.Left);
            SetTop(Selection.Rectangle, Selection.Top);

            SizeLabel.Content = Selection.Label;

            if (_dragging || _moving)
            {
                UpdateOpacityRectangles();
            }

            if (Selection.Invalid || _dragging)
            {
                SetPanelsVisibility(Visibility.Hidden);
            }
            else
            {
                SetPanelsVisibility(Visibility.Visible);
            }

            _events?.Dispatch<OnSelectionChange>(OnSelectionChangeArgs.From(Selection));
        }

        private void ClearOpacityRectangles()
        {
            foreach (var rectangle in _rectangles)
                Children.Remove(rectangle);

            _rectangles.Clear();
        }

        private void SetPanelsVisibility(Visibility visibility)
        {
            RightPanel.Visibility = visibility;
            BottomPanel.Visibility = visibility;
        }

        private void UpdateOpacityRectangles()
        {
            if (Selection is null) return;

            ClearOpacityRectangles();

            OpacityRectangle.Visibility = Visibility.Hidden;

            foreach (var bound in RectangleBounds.GetBounds(Selection, Width, Height))
            {
                (var rectangle, double left, double top) = bound;

                rectangle.MouseLeftButtonUp += RectangleOnMouseLeftButtonUp;
                rectangle.MouseLeftButtonDown += RectangleOnMouseLeftButtonDown;

                Children.Add(rectangle);

                SetLeft(rectangle, left);
                SetTop(rectangle, top);

                _rectangles.Insert(0, rectangle);
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            MouseLeftButtonPress(e);
        }

        private void RectangleOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseLeftButtonPress(e);
        }

        private void MouseLeftButtonPress(MouseButtonEventArgs e)
        {
            if (Tool != CanvasTool.Default)
                return;

            if (Selection is null)
            {
                Selection = Selection.Empty;

                Children.Add(Selection.Rectangle);
            }

            if (e.ClickCount >= 2)
            {
                Selection.Clear();
                Children.Remove(Selection.Rectangle);

                Selection = null;

                UpdateSelection();

                return;
            }

            var position = e.GetPosition(this);

            _moving = Selection.Contains(position);
            _dragging = !_moving;
            _startPosition = position;
            _tmpCursorPosition = _startPosition;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

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
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var newPosition = e.GetPosition(this);

            Cursor = GetCursor(newPosition);

            if (_moving)
                MoveSelection(newPosition);
            else if (_dragging) ResizeSelection(newPosition);

            UpdateSelection();
        }

        private Cursor GetCursor(Point point)
        {
            if (Selection is null)
                return Cursors.Arrow;

            if (Selection.Contains(point) && Tool == CanvasTool.Default)
                return Cursors.SizeAll;


            return Tool == CanvasTool.Default ? Cursors.Arrow : DrawingCursor;
        }

        private void ResizeSelection(Point cursorPosition)
        {
            if (_startPosition is not { } startPosition || Selection is null) return;

            (double left, double top) = PointUtils.GetCoords(startPosition, cursorPosition);

            Selection.Left = (int) left;
            Selection.Top = (int) top;
            Selection.Width = (int) Math.Abs(startPosition.X - cursorPosition.X);
            Selection.Height = (int) Math.Abs(startPosition.Y - cursorPosition.Y);
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
            var args = e.GetArgs<KeyEventArgs>();

            base.OnKeyDown(args);

            if (Selection == null) return;

            switch (args.Key)
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

            if (new[]
            {
                Key.Up, Key.Right, Key.Down, Key.Left
            }.Contains(args.Key))
            {
                UpdateSelection();
                UpdateOpacityRectangles();
            }
        }
    }
}