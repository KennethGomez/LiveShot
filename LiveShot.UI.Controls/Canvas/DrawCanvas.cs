using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LiveShot.API;
using LiveShot.API.Canvas;
using LiveShot.API.Drawing;
using LiveShot.API.Drawing.Tools;
using LiveShot.API.Events;
using LiveShot.API.Events.Input;
using LiveShot.API.Utils;

namespace LiveShot.UI.Controls.Canvas
{
    public class DrawCanvas : AbstractDrawCanvas
    {
        public static readonly DependencyProperty DrawingColorProperty = DependencyProperty.Register(
            "DrawingColor", typeof(Brush), typeof(DrawCanvas), new PropertyMetadata(Brushes.Red)
        );

        private readonly IList<UIElement> _history;
        private readonly Cursor[] _highlightCursors;

        private readonly Cursor _eyeDropperCursor =
            GetCursor<Rectangle>(Brushes.White, 1, false, 1, new Point(0.5, 0.5));

        private Cursor[] _cursors;
        private int _drawingStrokeThickness = 1;

        private ICollection<IDrawingTool>? _drawingTools;
        private IEventPipeline? _events;

        public override Cursor DrawingCursor => Tool switch
        {
            CanvasTool.EyeDropper => _eyeDropperCursor,
            CanvasTool.Highlight => _highlightCursors[_drawingStrokeThickness - 1],
            _ => _cursors[_drawingStrokeThickness - 1]
        };

        public override CanvasTool Tool { get; set; } = CanvasTool.Default;

        public override Brush DrawingColor
        {
            get => (Brush)GetValue(DrawingColorProperty);
            set
            {
                SetValue(DrawingColorProperty, value);

                _cursors = GetCursors();
            }
        }

        public override double DrawingStrokeThickness
        {
            get => _drawingStrokeThickness;
            set
            {
                _drawingStrokeThickness = value >= 1
                    ? value >= 16
                        ? 16
                        : (int)value
                    : 1;

                GetCanvasTool()?.UpdateThickness(_drawingStrokeThickness);

                _events?.Dispatch<OnCursorUpdate>(null);
            }
        }

        public DrawCanvas()
        {
            _history = new List<UIElement>();
            _cursors = GetCursors();
            _highlightCursors = GetHighlightCursors();
        }

        public void With(IEnumerable<IDrawingTool> tools, IEventPipeline events)
        {
            _drawingTools = tools.ToList();
            _events = events;

            _events.Subscribe<OnKeyDown>(OnKeyDown);
            _events.Subscribe<OnMouseWheel>(OnMouseWheel);
        }

        private Cursor[] GetCursors()
        {
            var cursors = new Cursor[16];

            for (var i = 0; i < cursors.Length; i++)
            {
                cursors[i] = GetCursor<Ellipse>(DrawingColor, i);
            }

            return cursors;
        }

        private static Cursor[] GetHighlightCursors()
        {
            var cursors = new Cursor[16];

            for (var i = 0; i < cursors.Length; i++)
            {
                cursors[i] = GetCursor<Ellipse>(HighlightTool.Color, i, true, HighlightTool.Opacity);
            }

            return cursors;
        }

        private static Cursor GetCursor<T>(
            Brush color, int size, bool fill = false, double opacity = 1, Point? point = null
        ) where T : Shape, new()
        {
            double ellipseSize = Math.Round((size + 1) * 1.3 + 2.25);

            T ellipse = new()
            {
                Width = ellipseSize,
                Height = ellipseSize,
                StrokeThickness = fill ? 0 : 1,
                Stroke = fill ? Brushes.Transparent : color,
                Opacity = opacity,
                Fill = fill ? color : Brushes.Transparent
            };

            return CursorUtils.GetCursorFromElement(ellipse, point ?? new Point(0.5, 0.5));
        }

        private IDrawingTool? GetCanvasTool(CanvasTool? tool = null)
        {
            return _drawingTools?.FirstOrDefault(t => t.Tool == (tool ?? Tool));
        }

        public void Undo()
        {
            if (_history.LastOrDefault() is not { } last) return;

            Children.Remove(last);
            _history.Remove(last);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (GetCanvasTool()?.OnMouseLeftButtonDown(e) is { } element)
                _history.Add(element);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (GetCanvasTool()?.OnMouseLeftButtonUp(e) is { } element)
                _history.Add(element);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (GetCanvasTool()?.OnMouseMove(e) is { } element)
                _history.Add(element);
        }

        private void OnKeyDown(Event e)
        {
            if (KeyBoardUtils.IsCtrlPressed)
            {
                switch (e.GetArgs<KeyEventArgs>().Key)
                {
                    case Key.Add:
                    case Key.OemPlus:
                        DrawingStrokeThickness++;
                        break;
                    case Key.Subtract:
                    case Key.OemMinus:
                        DrawingStrokeThickness--;
                        break;
                }
            }
        }

        private void OnMouseWheel(Event e)
        {
            if (e.GetArgs<MouseWheelEventArgs>().Delta > 0)
            {
                DrawingStrokeThickness--;
            }
            else
            {
                DrawingStrokeThickness++;
            }
        }

        public void SelectTool(CanvasTool tool)
        {
            GetCanvasTool(tool)?.Select();
        }

        public void UnselectTool(CanvasTool? tool = null)
        {
            GetCanvasTool(tool ?? Tool)?.Unselect();
        }

        public void Reset()
        {
            UnselectTool();

            Children.Clear();

            DrawingStrokeThickness = 1;

            Tool = CanvasTool.Default;
        }
    }
}