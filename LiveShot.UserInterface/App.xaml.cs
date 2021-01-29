using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LiveShot.UserInterface.ViewModels;
using LiveShot.UserInterface.Views;

namespace LiveShot.UserInterface
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new CaptureScreenWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}