using System;
using System.Windows;
using LiveShot.UI.Forms;

namespace LiveShot.UI
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Avalonia.Startup.Main(Array.Empty<string>());

            var captureScreenWindow = new CaptureScreenWindow();
            captureScreenWindow.Show();
        }
    }
}