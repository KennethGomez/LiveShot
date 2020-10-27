using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LiveShot.UI.Avalonia.Forms
{
    public class ExportWindow : Window
    {
        public ExportWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}