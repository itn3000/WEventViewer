using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Mono.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
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
    static bool ShowHelp = false;
    static OptionSet CreateOptionSet(OpenLogWindowViewModel vm)
    {
        var set = new OptionSet()
            .Add("n=|logname=", "LogName or Exported EventLog file path(*.evtx)", x => vm.LogName = x)
            .Add("t=|logtype=", "LogName kind, 'logname': from Windows EventLog store, 'filepath': exported Windows Event Log file(*.evtx) ", x =>
            {
                vm.CurrentSelected = x.ToLower() switch
                {
                    "logname" => vm.PathTypes[0],
                    "filepath" => vm.PathTypes[1],
                    _ => throw new ArgumentException("invalid logtype")
                };
            })
            .Add("l=|loglevel=", "log severity: availables = critical,error,warning,information,verbose", x =>
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
            .Add("p=|provider=", "log provider name", x => vm.ProviderNames = x)
            .Add("b=|begin=", "log begin datetime", x =>
            {
                DateTime dt = DateTime.Parse(x);
                vm.BeginDate = dt.ToString("yyyy-MM-dd");
                vm.BeginTime = dt.ToString("HH:mm:ss");
                vm.UseTimeCreated = true;
            })
            .Add("e=|end=", "log end datetime", x =>
            {
                DateTime dt = DateTime.Parse(x);
                vm.EndDate = dt.ToString("yyyy-MM-dd");
                vm.EndTime = dt.ToString("HH:mm:ss");
                vm.UseTimeCreated = true;
            })
            .Add("r=|raw=", "log filtering query by raw filter string", x =>
            {
                vm.RawQuery = x;
                vm.UseRawQuery = true;
            })
            .Add("h|help", x => ShowHelp = true)
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
        collection.AddSingleton<OpenLogWindowViewModel>();
        //if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime classic)
        //{
        //    collection.AddSingleton<OpenLogWindowViewModel>(provider =>
        //    {
        //        var vm = new OpenLogWindowViewModel();
        //        if (classic.Args != null)
        //        {
        //            var optset = CreateOptionSet(vm);
        //            var remaining = optset.Parse(classic.Args);
        //            if(remaining.Count > 0)
        //            {
        //                vm.LogName = remaining[0];
        //            }
        //        }
        //        return vm;
        //    });
        //}
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
        collection.AddTransient<HelpWindowViewModel>(provider =>
        {
            var optset = CreateOptionSet(provider.GetRequiredService<OpenLogWindowViewModel>());
            using var sw = new StringWriter();
            optset.WriteOptionDescriptions(sw);
            return new HelpWindowViewModel() { HelpMessage = sw.ToString() };
        });
        collection.AddTransient<HelpWindow>(provider => new HelpWindow() { DataContext = provider.GetRequiredService<HelpWindowViewModel>() });
        var serviceProvider = collection.BuildServiceProvider();
        //var vm = serviceProvider.GetRequiredService<MainWindowViewModel>();
        var w = serviceProvider.GetRequiredService<MainWindow>();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var openLogVM = serviceProvider.GetRequiredService<OpenLogWindowViewModel>();
            var optset = CreateOptionSet(openLogVM);
            if (desktop.Args != null)
            {
                var remaining = optset.Parse(desktop.Args);
                if(remaining.Count > 0)
                {
                    openLogVM.LogName = remaining[0];
                }
            }
            if (ShowHelp)
            {
                desktop.MainWindow = serviceProvider.GetRequiredService<HelpWindow>();
            }
            else
            {
                desktop.MainWindow = w;
            }
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = serviceProvider.GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}