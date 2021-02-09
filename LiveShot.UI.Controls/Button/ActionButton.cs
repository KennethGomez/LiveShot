using System.Windows;
using System.Windows.Media;
using LiveShot.UI.Controls.Canvas;

namespace LiveShot.UI.Controls.Button
{
    public class ActionButton : System.Windows.Controls.Button
    {
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            "IsActive", typeof(bool), typeof(ActionButton), new PropertyMetadata(false)
        );

        public static readonly DependencyProperty IconFillProperty = DependencyProperty.Register(
            "IconFill", typeof(Brush), typeof(ActionButton), new PropertyMetadata(Brushes.Black)
        );

        public static readonly DependencyProperty ActiveActionProperty = DependencyProperty.Register(
            "ActiveAction", typeof(CanvasAction), typeof(ActionButton), new PropertyMetadata(CanvasAction.Default)
        );

        public bool IsActive
        {
            get => (bool) GetValue(IsActiveProperty);
            set
            {
                SetValue(IsActiveProperty, value);
                
                IconFill = value ? Brushes.MediumBlue : Brushes.Black;
            }
        }

        public Brush IconFill
        {
            get => (Brush) GetValue(IconFillProperty);
            private set => SetValue(IconFillProperty, value);
        }

        public CanvasAction ActiveAction
        {
            get => (CanvasAction) GetValue(ActiveActionProperty);
            set => SetValue(ActiveActionProperty, value);
        }
    }
}