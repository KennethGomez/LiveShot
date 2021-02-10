using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LiveShot.API.Drawing;

namespace LiveShot.UI.Controls.Canvas
{
    public class DrawCanvas : System.Windows.Controls.Canvas
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color", typeof(Brush), typeof(DrawCanvas), new PropertyMetadata(Brushes.Red)
        );

        private readonly IDictionary<int, (int, ICollection<UIElement>)> _history;

        private ICollection<IDrawingTool>? _drawingTools;

        public CanvasTool Tool = CanvasTool.Default;

        protected DrawCanvas()
        {
            _history = new Dictionary<int, (int, ICollection<UIElement>)>();
        }

        public Brush Color
        {
            get => (Brush) GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public void WithDrawingTools(IEnumerable<IDrawingTool> tools)
        {
            _drawingTools = tools.ToList();
        }

        private IDrawingTool? GetCanvasTool()
        {
            var tool = _drawingTools?.FirstOrDefault(t => t.Tool == Tool);

            if (tool is null) return null;

            tool.Color = Color;

            return tool;
        }

        public void Undo()
        {
            if (_history.Count == 0) return;

            (int historyIndex, (int i, var uiElements)) = _history.LastOrDefault();

            Children.RemoveRange(i, uiElements.Count);
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