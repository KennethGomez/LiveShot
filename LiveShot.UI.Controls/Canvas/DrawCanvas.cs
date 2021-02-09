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
        private ICollection<IDrawingTool>? _drawingTools; 
        
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color", typeof(Brush), typeof(DrawCanvas), new PropertyMetadata(Brushes.Red)
        );
        
        public CanvasTool Tool = CanvasTool.Default;

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

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            GetCanvasTool()?.OnMouseLeftButtonDown(e, this);
        }
        
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            GetCanvasTool()?.OnMouseLeftButtonUp(e, this);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            GetCanvasTool()?.OnMouseMove(e, this);
        }
    }
}