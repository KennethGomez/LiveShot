using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LiveShot.UI.Controls
{
    public class SelectCanvas : Canvas
    {
        private Point? _startPosition;

        private Rectangle _selection;

        private bool _dragging = true;

        private double SelectionLeft
        {
            get => _selection != null ? GetLeft(_selection) : 0;
            set
            {
                if (_selection != null && value > 0)
                {
                    SetLeft(_selection, value);
                }
            }
        }

        private double SelectionTop
        {
            get => _selection != null ? GetTop(_selection) : 0;
            set
            {
                if (_selection != null && value > 0)
                {
                    SetTop(_selection, value);
                }
            }
        }
        
        private double SelectionWidth
        {
            get => _selection?.Width ?? 0;
            set
            {
                if (_selection != null && value > 0)
                {
                    _selection.Width = value;
                }
            }
        }
        
        private double SelectionHeight
        {
            get => _selection?.Height ?? 0;
            set
            {
                if (_selection != null && value > 0)
                {
                    _selection.Height = value;
                }
            }
        }

        public SelectCanvas()
        {
            Background = Brushes.Black;
            Opacity = .2;
        }

        private static void OnSelectionKeyDown(KeyEventArgs e, Key key, Action<int> shiftPressed, Action<int> normal)
        {
            if (e.Key != key) return;

            bool controlDown = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            int step = controlDown ? 10 : 1;

            bool shiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (shiftDown)
            {
                shiftPressed(step);
            }
            else
            {
                normal(step);
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            bool exists = _selection != null;

            if (!exists)
            {
                _selection = new Rectangle
                {
                    Fill = Brushes.White,
                    Opacity = 1
                };

                Children.Add(_selection);
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

            SelectionLeft = rectLeft;
            SelectionTop = rectTop;
            SelectionWidth = Math.Abs(xDiff);
            SelectionHeight = Math.Abs(yDiff);
        }

        public void ParentKeyDown(KeyEventArgs e)
        {
            if (_selection == null) return;

            OnSelectionKeyDown(e, Key.Up, n => SelectionHeight -= n, n => SelectionTop -= n);
            OnSelectionKeyDown(e, Key.Right, n => SelectionWidth += n, n => SelectionLeft += n);
            OnSelectionKeyDown(e, Key.Down, n => SelectionHeight += n, n => SelectionTop += n);
            OnSelectionKeyDown(e, Key.Left, n => SelectionWidth -= n, n => SelectionLeft -= n);
        }
    }
}