using System;
using System.Windows;
using LiveShot.Objects;
using LiveShot.Objects.Events.Window;

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
            
            EventPipeline.Dispatch<OnClosed>(new OnClosedArgs
            {
                Root = e,
                Window = this
            });
        }
    }
}