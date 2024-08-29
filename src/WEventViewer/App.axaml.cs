using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata.Ecma335;
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
        collection.AddSingleton<IViewModelFactory, ViewModelFactoryServiceProvider>(provider => new ViewModelFactoryServiceProvider(provider));
        collection.AddSingleton<MainWindowViewModel>();
        collection.AddSingleton<OpenLogWindowViewModel>();
        collection.AddSingleton<MainWindow>(provider =>
        {
            return new MainWindow(provider.GetRequiredService<IViewModelFactory>())
            {
                DataContext = provider.GetRequiredService<MainWindowViewModel>()
            };
        });
        collection.AddTransient<ErrorWindowViewModel>();
        collection.AddTransient<ErrorWindow>();
        collection.AddTransient<OpenLogWindow>();
        collection.AddTransient<ProviderNameWindowViewModel>();
        collection.AddTransient<LogNameViewModel>();
        collection.AddTransient<AboutViewModel>();
        var serviceProvider = collection.BuildServiceProvider();
        var vm = serviceProvider.GetRequiredService<MainWindowViewModel>();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = serviceProvider.GetRequiredService<MainWindow>();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = serviceProvider.GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}