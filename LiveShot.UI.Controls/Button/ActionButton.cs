using System.Windows;
using System.Windows.Media;
using LiveShot.API.Controls.Button;
using LiveShot.API.Drawing;
using LiveShot.API.Drawing.Tools;

namespace LiveShot.UI.Controls.Button
{
    public class ActionButton : System.Windows.Controls.Button, IActionButton
    {
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            "IsActive", typeof(bool), typeof(ActionButton), new PropertyMetadata(false)
        );

        public static readonly DependencyProperty IconFillProperty = DependencyProperty.Register(
            "IconFill", typeof(Brush), typeof(ActionButton), new PropertyMetadata(Brushes.Black)
        );

        public static readonly DependencyProperty ActiveToolProperty = DependencyProperty.Register(
            "ActiveTool", typeof(CanvasTool), typeof(ActionButton), new PropertyMetadata(CanvasTool.Default)
        );

        public bool IsActive
        {
            get => (bool) GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public Brush IconFill
        {
            get => (Brush) GetValue(IconFillProperty);
            set => SetValue(IconFillProperty, value);
        }

        public CanvasTool ActiveTool
        {
            get => (CanvasTool) GetValue(ActiveToolProperty);
            set => SetValue(ActiveToolProperty, value);
        }

        public void UpdateIconFill(Brush brush)
        {
            if (ActiveTool == CanvasTool.Highlight)
                brush = HighlightTool.Color;
            
            IconFill = IsActive ? brush : Brushes.Black;
        }
    }
}