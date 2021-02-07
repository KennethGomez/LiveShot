using System;
using System.Diagnostics;
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

            OpenBtn.Click += OpenBtnOnClick;
            CopyBtn.Click += CopyBtnOnClick;
        }

        private void CopyBtnOnClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(LinkBox.Text);
        }

        private void OpenBtnOnClick(object sender, RoutedEventArgs e)
        {
            OpenUrl(LinkBox.Text);
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch (Exception)
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") {CreateNoWindow = true});
            }
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

        public void Upload(Bitmap bitmap, bool google)
        {
            Dispatcher.BeginInvoke(async () =>
            {
                try
                {
                    LinkBox.Text = await _uploadService.Upload(bitmap);

                    UploadResultGrid.Visibility = Visibility.Visible;
                    ProgressBarGrid.Visibility = Visibility.Hidden;

                    if (google)
                    {
                        OpenUrl($"https://www.google.com/searchbyimage?image_url={LinkBox.Text}");
                        Close();
                    }
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