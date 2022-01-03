using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using LiveShot.API;
using LiveShot.API.Background;
using LiveShot.API.Events.Application;
using LiveShot.API.Events.Capture;
using LiveShot.API.Utils;
using LiveShot.UI.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LiveShot.UI
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private IServiceProvider? ServiceProvider { get; set; }

        private IConfiguration? Configuration { get; set; }

        private CaptureScreenView? CaptureScreenView { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            LoadConfiguration();
            SetUICulture();

            var serviceCollection = new ServiceCollection()
                .ConfigureAPI(Configuration)
                .ConfigureUI();

            if (Configuration != null)
                serviceCollection.AddSingleton(Configuration);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            CaptureScreenView = ServiceProvider.GetRequiredService<CaptureScreenView>();

            if (e.Args.Contains("--background"))
            {
                StartBackgroundApp();

                Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            }
            else
            {
                StartCaptureScreenShot();
            }
        }

        private void StartCaptureScreenShot()
        {
            if (WindowUtils.IsOpen(typeof(CaptureScreenView))) return;

            if (CaptureScreenView is null) 
                return;

            CaptureScreenView.CaptureScreen();
            CaptureScreenView.Show();
            CaptureScreenView.Activate();
        }

        private void StartBackgroundApp()
        {
            ServiceProvider?.GetRequiredService<IBackgroundApplication>().Init();

            var eventPipeline = ServiceProvider?.GetRequiredService<IEventPipeline>();

            eventPipeline?.Subscribe<CaptureScreenShotEvent>(_ => StartCaptureScreenShot());
            eventPipeline?.Subscribe<ShutdownApplicationEvent>(_ => Shutdown());
        }

        private void SetUICulture()
        {
            string? culture = Configuration?["CultureUI"];

            if (culture is not null) Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
        }

        private void LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Properties/appsettings.json", false, true);

            Configuration = builder.Build();
        }
    }
}