using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LiveShot.API.Canvas;

namespace LiveShot.API.Drawing.Tools
{
    public class TextTool : DrawingTool
    {
        public override CanvasTool Tool => CanvasTool.Text;

        private TextBox? _text;

        public override void OnMouseLeftButtonDown(MouseButtonEventArgs e, AbstractDrawCanvas canvas)
        {
            if (e.ButtonState != MouseButtonState.Pressed) return;

            var point = e.GetPosition(canvas);

            LastPoint = point;
            
            _text = new TextBox
            {
                FontSize = GetFontSize(canvas.DrawingStrokeThickness),
                Foreground = canvas.DrawingColor,
                CaretBrush = canvas.DrawingColor,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0)
            };
            
            System.Windows.Controls.Canvas.SetLeft(_text, point.X);
            System.Windows.Controls.Canvas.SetTop(_text, point.Y);

            canvas.Children.Add(_text);

            _text.Focus();
            _text.KeyDown += TextOnKeyDown;
            _text.LostFocus += TextOnLostFocus;
        }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e, AbstractDrawCanvas canvas) { }

        public override void OnMouseMove(MouseEventArgs e, AbstractDrawCanvas canvas) { }

        public override void UpdateThickness(double thickness)
        {
            if (_text is not null)
                _text.FontSize = GetFontSize(thickness);
        }

        private void TextOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is TextBox textBox)
            {
                LoseTextBoxFocus(textBox);
            }
        }

        private void TextOnLostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                LoseTextBoxFocus(textBox);
            }
        }

        private void LoseTextBoxFocus(UIElement textBox)
        {
            textBox.IsHitTestVisible = false;

            Keyboard.ClearFocus();

            if (_text == textBox)
                _text = null;
        }

        private static double GetFontSize(double thickness)
        {
            return thickness * 1.1 + 14;
        }
    }
}