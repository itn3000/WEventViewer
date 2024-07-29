using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using WEventViewer.Model;
using WEventViewer.ViewModel;

namespace WEventViewer;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.AddSingleton<EventLogRepository>();
        collection.AddSingleton<MainWindowViewModel>(provider => new MainWindowViewModel(provider.GetRequiredService<EventLogRepository>()));
        collection.AddTransient<ErrorWindow>();
        collection.AddTransient<ErrorWindowViewModel>();
        collection.AddTransient<OpenLogWindowViewModel>();
        collection.AddTransient<OpenLogWindow>();
        collection.AddTransient<ProviderNameWindowViewModel>();
        collection.AddTransient<LogNameViewModel>();
        var serviceProvider = collection.BuildServiceProvider();
        var vm = serviceProvider.GetRequiredService<MainWindowViewModel>();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow(serviceProvider) { DataContext = vm };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainWindow(serviceProvider) { DataContext = vm }; ;
        }

        base.OnFrameworkInitializationCompleted();
    }
}