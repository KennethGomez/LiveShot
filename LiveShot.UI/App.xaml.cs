using System;
using System.Globalization;
using System.IO;
using System.Windows;
using LiveShot.API;
using LiveShot.UI.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LiveShot.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider? ServiceProvider { get; set; }

        private IConfiguration? Configuration { get; set; }
 
        protected override void OnStartup(StartupEventArgs e)
        {
            LoadConfiguration();
            SetUICulture();

            var serviceCollection = new ServiceCollection()
                .ConfigureAPI()
                .ConfigureUI();

            if (Configuration != null) 
                serviceCollection.AddSingleton(Configuration);

            ServiceProvider = serviceCollection.BuildServiceProvider();
            ServiceProvider.GetRequiredService<CaptureScreenView>().Show();
        }

        private void SetUICulture()
        {
            string? culture = Configuration?["CultureUI"];

            if (culture is not null)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
            }
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