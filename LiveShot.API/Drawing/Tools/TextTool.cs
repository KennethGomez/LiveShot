using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LiveShot.API.Drawing.Tools
{
    public class TextTool : DrawingTool
    {
        private TextBox? _text;

        private readonly ILiveShotService _liveShotService;

        public TextTool(ILiveShotService liveShotService)
        {
            _liveShotService = liveShotService;
        }

        public override CanvasTool Tool => CanvasTool.Text;

        public override UIElement? OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_liveShotService.DrawCanvas is not { } canvas) return null;

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

            return _text;
        }

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
            textBox.KeyDown -= TextOnKeyDown;
            textBox.LostFocus -= TextOnLostFocus;

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