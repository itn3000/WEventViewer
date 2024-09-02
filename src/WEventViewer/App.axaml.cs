using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Mono.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Reflection.Metadata.Ecma335;
using WEventViewer.Model;
using WEventViewer.ViewModel;

namespace WEventViewer;

public partial class App : Application
{
    class WEventViewOptions
    {
        public string LogName = "";
        public string LogType = "";
        public List<int> LogLevels = new List<int>();
    }
    static OptionSet CreateOptionSet(OpenLogWindowViewModel vm)
    {
        var set = new OptionSet()
            .Add("n=|logname=", x => vm.LogName = x)
            .Add("t=|logtype=", x =>
            {
                vm.CurrentSelected = x.ToLower() switch
                {
                    "logname" => vm.PathTypes[0],
                    "filepath" => vm.PathTypes[1],
                    _ => throw new ArgumentException("invalid logtype")
                };
            })
            .Add("l=|loglevel=", x =>
            {
                switch(x.ToLower())
                {
                    case "critical":
                        vm.IsCriticalChecked = true;
                        break;
                    case "error":
                        vm.IsErrorChecked = true;
                        break;
                    case "warning":
                        vm.IsWarningChecked = true;
                        break;
                    case "information":
                        vm.IsInformationChecked = true;
                        break;
                    case "verbose":
                        vm.IsVerboseChecked = true;
                        break;
                }
                vm.UseFilterByLevel = true;
            })
            .Add("p=|provider=", x => vm.ProviderNames = x)
            .Add("b=|begin=", x =>
            {
                DateTime dt = DateTime.Parse(x);
                vm.BeginDate = dt.ToString("yyyy-MM-dd");
                vm.BeginTime = dt.ToString("HH:mm:ss");
                vm.UseTimeCreated = true;
            })
            .Add("e=|end=", x =>
            {
                DateTime dt = DateTime.Parse(x);
                vm.EndDate = dt.ToString("yyyy-MM-dd");
                vm.EndTime = dt.ToString("HH:mm:ss");
                vm.UseTimeCreated = true;
            })
            .Add("r=|raw=", x =>
            {
                vm.RawQuery = x;
                vm.UseRawQuery = true;
            })
            ;
        return set;
    }
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
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime classic)
        {
            collection.AddSingleton<OpenLogWindowViewModel>(provider =>
            {
                var vm = new OpenLogWindowViewModel();
                if (classic.Args != null)
                {
                    var optset = CreateOptionSet(vm);
                    optset.Parse(classic.Args);
                }
                return vm;
            });
        }
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
        collection.AddTransient<DetailedLogViewModel>();
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