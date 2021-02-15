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

        private readonly IDictionary<int, (int, ICollection<UIElement>)> _history;
        private Cursor[] _cursors;

        private int _drawingStrokeThickness = 1;

        private ICollection<IDrawingTool>? _drawingTools;
        private IEventPipeline? _events;

        public override Cursor DrawingCursor => _cursors[_drawingStrokeThickness - 1];
        public override CanvasTool Tool { get; set; } = CanvasTool.Default;

        public override double DrawingStrokeThickness
        {
            get => _drawingStrokeThickness;
            set
            {
                _drawingStrokeThickness = value >= 1
                    ? value >= 16
                        ? 16
                        : (int) value
                    : 1;
                
                GetCanvasTool()?.UpdateThickness(_drawingStrokeThickness);
                
                _events?.Dispatch<OnCursorUpdate>(null);
            }
        }

        public DrawCanvas()
        {
            _history = new Dictionary<int, (int, ICollection<UIElement>)>();
            _cursors = GetCursors();
        }

        public override Brush DrawingColor
        {
            get => (Brush) GetValue(DrawingColorProperty);
            set
            {
                SetValue(DrawingColorProperty, value);

                _cursors = GetCursors();
            }
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
                Ellipse ellipse = new()
                {
                    Width = Math.Round((i + 1) * 1.3 + 2.25),
                    Height = Math.Round((i + 1) * 1.3 + 2.25),
                    Stroke = DrawingColor,
                    Opacity = 1
                };

                cursors[i] = CursorUtils.GetCursorFromElement(ellipse, new Point(0.5, 0.5));
            }
            
            return cursors;
        }

        private IDrawingTool? GetCanvasTool()
        {
            return _drawingTools?.FirstOrDefault(t => t.Tool == Tool);
        }

        public override void Undo()
        {
            if (_history.Count == 0) return;

            (int historyIndex, (_, var uiElements)) = _history.LastOrDefault();

            foreach (var element in uiElements)
            {
                Children.Remove(element);
            }

            _history.Remove(historyIndex);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _history[_history.Count] = (Children.Count, new List<UIElement>());

            GetCanvasTool()?.OnMouseLeftButtonDown(e, this);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            GetCanvasTool()?.OnMouseLeftButtonUp(e, this);

            (int startIndex, var uiElements) = _history.LastOrDefault().Value;

            for (int i = startIndex; i < Children.Count; i++)
                uiElements.Add(Children[i]);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            GetCanvasTool()?.OnMouseMove(e, this);
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
            if (KeyBoardUtils.IsCtrlPressed)
            {
                if (e.GetArgs<MouseWheelEventArgs>().Delta > 0)
                {
                    DrawingStrokeThickness++;
                }
                else
                {
                    DrawingStrokeThickness--;
                }
            }
        }
    }
}