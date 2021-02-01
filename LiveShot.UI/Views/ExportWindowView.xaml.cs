using System;
using System.Windows;
using LiveShot.API;
using LiveShot.API.Events.Window;

namespace LiveShot.UI.Views
{
    public partial class ExportWindowView : Window
    {
        private readonly IEventPipeline _events;
        
        public ExportWindowView(IEventPipeline events)
        {
            InitializeComponent();

            _events = events;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            
            _events.Dispatch<OnClosed>(new OnClosedArgs
            {
                Root = e,
                Window = this
            });
        }
    }
}