using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LiveShot.API.Canvas;
using LiveShot.API.Drawing;
using LiveShot.API.Utils;

namespace LiveShot.UI.Controls.Canvas
{
    public class DrawCanvas : AbstractDrawCanvas
    {
        public static readonly DependencyProperty DrawingColorProperty = DependencyProperty.Register(
            "DrawingColor", typeof(Brush), typeof(DrawCanvas), new PropertyMetadata(Brushes.Red)
        );

        private readonly IDictionary<int, (int, ICollection<UIElement>)> _history;

        private ICollection<IDrawingTool>? _drawingTools;

        private Ellipse CursorEllipse => new()
        {
            Width = DrawingStrokeThickness * 4,
            Height = DrawingStrokeThickness * 4,
            Stroke = DrawingColor,
            Opacity = 1
        };

        public override Cursor DrawingCursor => CursorUtils.GetCursorFromElement(CursorEllipse, new Point(0.5, 0.5));
        public override CanvasTool Tool { get; set; } = CanvasTool.Default;

        public double DrawingStrokeThickness = 1;

        public DrawCanvas()
        {
            _history = new Dictionary<int, (int, ICollection<UIElement>)>();
        }

        public override Brush DrawingColor
        {
            get => (Brush) GetValue(DrawingColorProperty);
            set => SetValue(DrawingColorProperty, value);
        }

        public void WithDrawingTools(IEnumerable<IDrawingTool> tools)
        {
            _drawingTools = tools.ToList();
        }

        private IDrawingTool? GetCanvasTool()
        {
            return _drawingTools?.FirstOrDefault(t => t.Tool == Tool);
        }

        public override void Undo()
        {
            if (_history.Count == 0) return;

            (int historyIndex, (int i, var uiElements)) = _history.LastOrDefault();

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
    }
}