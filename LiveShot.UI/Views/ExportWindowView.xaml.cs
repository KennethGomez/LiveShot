using System;
using System.Drawing;
using System.Windows;
using System.Windows.Threading;
using LiveShot.API;
using LiveShot.API.Events.Window;
using LiveShot.API.Upload;

namespace LiveShot.UI.Views
{
    public partial class ExportWindowView : Window
    {
        private readonly IEventPipeline _events;
        private readonly IUploadService _uploadService;

        public ExportWindowView(IEventPipeline events, IUploadService uploadService)
        {
            InitializeComponent();

            _events = events;
            _uploadService = uploadService;
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

        public void Upload(Bitmap bitmap)
        {
            Dispatcher.BeginInvoke(async () =>
            {
                try
                {
                    LinkBox.Text = await _uploadService.Upload(bitmap);

                    UploadResultGrid.Visibility = Visibility.Visible;
                    ProgressBarGrid.Visibility = Visibility.Hidden;
                }
                catch (Exception e)
                {
                    MessageBox.Show(
                        e.Message,
                        API.Properties.Resources.Exception_Message,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );

                    Close();
                }
            });
        }
    }
}