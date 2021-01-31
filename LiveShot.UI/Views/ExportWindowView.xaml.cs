using System;
using System.Windows;

namespace LiveShot.UI.Views
{
    public partial class ExportWindowView : Window
    {
        public ExportWindowView()
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
    }
}